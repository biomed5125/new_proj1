//// Features/Communication/Bench/BenchAnalyzerDriver.cs
//using System;
//using System.Collections.Concurrent;
//using System.Globalization;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using HMS.Communication.Abstractions;
//using HMS.Communication.Infrastructure.Persistence;   // CommunicationDbContext
//using HMS.Communication.Persistence;
//using HMS.Module.Lab.Features.Lab.Models.Entities;    // myLabRequestItem, myLabResult, ...
//using HMS.Module.Lab.Features.Lab.Models.Enums;       // LabResultStatus
//using HMS.Module.Lab.Infrastructure.Persistence;      // LabDbContext

//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;

//namespace HMS.Api.Features.Communication.Bench
//{
//    public sealed class BenchAnalyzerDriver : IAnalyzerDriver
//    {
//        private readonly CommunicationDbContext _comm;
//        private readonly LabDbContext _lab;
//        private readonly ILogger<BenchAnalyzerDriver> _log;

//        // Remember latest accession per device so R can attach if it doesn't carry accession itself.
//        private static readonly ConcurrentDictionary<long, string> _lastAccByDevice = new();

//        public BenchAnalyzerDriver(
//            CommunicationDbContext comm,
//            LabDbContext lab,
//            ILogger<BenchAnalyzerDriver> log)
//        {
//            _comm = comm;
//            _lab = lab;
//            _log = log;
//        }

//        public async Task HandleInboundAsync(InboundEnvelope env, CancellationToken ct)
//        {
//            var core = Core(env.RawFrame);
//            if (string.IsNullOrEmpty(core))
//                return;

//            var f = core.Split('|');
//            var rec = f[0];                    // "H","P","O","R","L",...

//            switch (rec)
//            {
//                case "O": // O|1|ACC-...|...
//                    {
//                        var acc = f.Length > 2 ? NullIfEmpty(f[2]) : null;
//                        if (!string.IsNullOrEmpty(acc))
//                        {
//                            _lastAccByDevice[env.DeviceId] = acc;
//                            await StampInbound(env, "O", acc, null, ct);
//                        }
//                        else
//                        {
//                            await StampInbound(env, "O", null, null, ct);
//                        }
//                        break;
//                    }

//                case "R": // R|1|^^^GLU^SC|105|mg/dL|70-110|N|F|...
//                    {
//                        var code = ParseInstrumentCodeFromR(f);     // e.g., GLU
//                        var (val, unit) = ParseValueAndUnitFromR(f);

//                        // Stamp trace even if incomplete, so you can see what arrived.
//                        await StampInbound(env, "R", null, code, ct);

//                        if (string.IsNullOrEmpty(code) || val is null)
//                        {
//                            _log.LogWarning("R record missing code/value. Core={Core}", core);
//                            break;
//                        }

//                        // accession from cache -> else fallback to DB (last O for this device)
//                        if (!_lastAccByDevice.TryGetValue(env.DeviceId, out var acc) || string.IsNullOrEmpty(acc))
//                            acc = await FindRecentAccessionFromDb(env.DeviceId, ct);

//                        if (string.IsNullOrEmpty(acc))
//                        {
//                            _log.LogWarning("No accession available for R record (device {Device}).", env.DeviceId);
//                            break;
//                        }

//                        // Update trace row with accession now that we have it
//                        await StampInbound(env, "R", acc, code, ct);

//                        await UpsertResultAsync(acc, code!, val.Value, unit, ct);
//                        break;
//                    }

//                default:
//                    // Optional: stamp H/P/L/… frames for visibility in the trace grid
//                    await StampInbound(env, rec, null, null, ct);
//                    break;
//            }
//        }

//        // ----------------- parsing helpers -----------------

//        // Extracts the content between STX..ETX (removes CR/LF), or returns trimmed if not framed
//        private static string Core(string raw)
//        {
//            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

//            int i = raw.IndexOf('\x02'); // STX
//            int j = raw.IndexOf('\x03'); // ETX

//            var s = (i >= 0 && j > i) ? raw.Substring(i + 1, j - i - 1) : raw;
//            s = s.Replace("\r", "").Replace("\n", "");
//            return s.Trim();
//        }

//        private static string? ParseInstrumentCodeFromR(string[] f)
//        {
//            if (f.Length <= 2) return null;
//            // R.3 = "^^^GLU^SC" -> component 4 = GLU
//            var comps = f[2].Split('^');
//            return comps.Length >= 4 ? NullIfEmpty(comps[3]) : null;
//        }

//        private static (decimal? value, string? unit) ParseValueAndUnitFromR(string[] f)
//        {
//            // Accept both:
//            //  - R|...|105|mg/dL|...
//            //  - R|...|"105 mg/dL"|...
//            decimal? value = null;
//            string? unit = null;

//            if (f.Length > 3 && !string.IsNullOrWhiteSpace(f[3]))
//            {
//                var raw = f[3].Trim();

//                // 105 or "105 mg/dL"
//                var firstToken = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
//                if (!string.IsNullOrEmpty(firstToken) &&
//                    decimal.TryParse(firstToken, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
//                {
//                    value = d;
//                }
//            }

//            if (f.Length > 4 && !string.IsNullOrWhiteSpace(f[4]))
//            {
//                unit = f[4].Trim();
//            }
//            else if (f.Length > 3)
//            {
//                // Try to pick unit from combined field "105 mg/dL"
//                var parts = f[3].Split(' ', StringSplitOptions.RemoveEmptyEntries);
//                if (parts.Length >= 2)
//                    unit = parts[1];
//            }

//            return (value, unit);
//        }

//        private static string? NullIfEmpty(string s) => string.IsNullOrWhiteSpace(s) ? null : s;

//        // ----------------- persistence helpers -----------------

//        private async Task<string?> FindRecentAccessionFromDb(long deviceId, CancellationToken ct)
//        {
//            // Look for most recent inbound O record for this device
//            var lastCore = await _comm.Inbound.AsNoTracking()
//                .Where(x => x.DeviceId == deviceId && x.ParsedRecordType == "O")
//                .OrderByDescending(x => x.Id)
//                .Select(x => Core(x.RawFrame))
//                .FirstOrDefaultAsync(ct);

//            if (string.IsNullOrEmpty(lastCore)) return null;

//            var f = lastCore.Split('|');
//            return (f.Length > 2 && f[0] == "O") ? NullIfEmpty(f[2]) : null;
//        }

//        private async Task StampInbound(InboundEnvelope env,string? recType,string? accession,string? instrCode,CancellationToken ct)
//        {
//            var rx = await _comm.Inbound
//                .Where(x => x.DeviceId == env.DeviceId && x.RawFrame == env.RawFrame)
//                .OrderByDescending(x => x.Id)
//                .FirstOrDefaultAsync(ct);

//            if (rx is null) return;

//            var entry = _comm.Entry(rx);

//            if (recType is not null) entry.Property(e => e.ParsedRecordType).CurrentValue = recType;
//            if (accession is not null) entry.Property(e => e.AccessionNumber).CurrentValue = accession;
//            if (instrCode is not null) entry.Property(e => e.InstrumentTestCode).CurrentValue = instrCode;

//            await _comm.SaveChangesAsync(ct);
//        }


//        private async Task UpsertResultAsync(string accession, string code, decimal value, string? unit, CancellationToken ct)
//        {
//            // 1) Find the sample by accession
//            var sample = await _lab.LabSamples
//                .AsNoTracking()
//                .FirstOrDefaultAsync(s => s.AccessionNumber == accession, ct);

//            if (sample is null)
//            {
//                _log.LogWarning("No LabSample for accession {Accession}", accession);
//                return;
//            }

//            // 2) Resolve LabTestId from instrument code mapping, fall back to LabTests.Code
//            long? labTestId = await _lab.InstrumentTestMaps
//                .Where(m => m.InstrumentTestCode == code)
//                .Select(m => (long?)m.LabTestId)
//                .FirstOrDefaultAsync(ct);

//            if (labTestId is null)
//            {
//                labTestId = await _lab.LabTests
//                    .Where(t => t.Code == code)
//                    .Select(t => (long?)t.LabTestId)
//                    .FirstOrDefaultAsync(ct);
//            }

//            if (labTestId is null)
//            {
//                _log.LogWarning("Unknown test code {Code} for accession {Accession}", code, accession);
//                return;
//            }

//            // 3) Find or create the request item
//            var item = await _lab.LabRequestItems
//                .FirstOrDefaultAsync(i => i.LabRequestId == sample.LabRequestId && i.LabTestId == labTestId.Value, ct);

//            if (item is null)
//            {
//                item = new myLabRequestItem
//                {
//                    LabRequestId = sample.LabRequestId,
//                    LabTestId = labTestId.Value,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedBy = "driver",
//                    IsDeleted = false
//                };
//                _lab.LabRequestItems.Add(item);
//                await _lab.SaveChangesAsync(ct); // ensure LabRequestItemId
//            }

//            // 4) Upsert result
//            var result = await _lab.LabResults
//                .FirstOrDefaultAsync(r => r.LabRequestItemId == item.LabRequestItemId, ct);

//            var valueText = value.ToString(CultureInfo.InvariantCulture);

//            if (result is null)
//            {
//                result = new myLabResult
//                {
//                    LabRequestItemId = item.LabRequestItemId,
//                    Value = valueText,
//                    Unit = unit,
//                    Status = (LabResultStatus)1,     // Completed
//                    CreatedAt = DateTime.UtcNow
//                };
//                _lab.LabResults.Add(result);
//            }
//            else
//            {
//                result.Value = valueText;
//                result.Unit = unit;
//                result.Status = (LabResultStatus)1;   // Completed
//                // If you keep audit fields, set UpdatedAt/By here
//            }

//            await _lab.SaveChangesAsync(ct);
//        }
//    }
//}

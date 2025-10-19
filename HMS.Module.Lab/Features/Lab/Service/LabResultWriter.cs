using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;        // ResultFlag, LabResultStatus, LabRequestStatus
using HMS.Module.Lab.Infrastructure.Persistence;
// LabDbContext
// LabRequest, LabRequestItem, LabSample, LabResult, InstrumentMap...
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Module.Lab.Features.Lab.Service
{
    public sealed class LabResultWriter : ILabResultWriter
    {
        private readonly LabDbContext _db;
        private readonly ILogger<LabResultWriter> _log;

        public LabResultWriter(LabDbContext db, ILogger<LabResultWriter> log)
        {
            _db = db;
            _log = log;
        }

    public async Task UpsertResultAsync(
    string accession,
    long deviceId,
    string instrumentTestCode,
    string? value,
    string? units,
    ResultFlag? flag,
    string? rawFlagOrNotes,
    CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(accession))
                throw new ArgumentException("Accession is required.", nameof(accession));

            accession = accession.Trim();

            // 1) Resolve the request through the SAMPLE (prevents FK errors)
            var sample = await _db.LabSamples
                .AsNoTracking()
                .Where(s => s.AccessionNumber == accession)
                .Select(s => new { s.LabSampleId, s.LabRequestId, s.AccessionNumber })
                .SingleOrDefaultAsync(ct);

            if (sample is null)
            {
                _log.LogWarning("UpsertResult: accession {acc} not found. Dropping result (device {dev}, code {code}).",
                    accession, deviceId, instrumentTestCode);
                return; // or write to a dead-letter table if you want
            }

            // 2) Map instrument test code -> LIS code (device-specific first; fallback to global; fallback to instrument)
            var lisCode = await _db.InstrumentTestMaps
                            .Where(m => m.InstrumentTestCode == instrumentTestCode &&
                                        (m.DeviceId == deviceId || m.DeviceId == 0))
                            .OrderByDescending(m => m.DeviceId == deviceId)
                            .Select(m => m.LabTestCode)
                            .FirstOrDefaultAsync(ct)
                         ?? instrumentTestCode;

            lisCode = lisCode?.Trim();
            if (string.IsNullOrWhiteSpace(lisCode))
            {
                _log.LogWarning("UpsertResult: could not resolve LIS code. Acc={acc}, Instr={instr}", accession, instrumentTestCode);
                return;
            }

            // 3) Ensure we have a request item for this LIS test code
            var item = await _db.LabRequestItems
                .FirstOrDefaultAsync(i => i.LabRequestId == sample.LabRequestId && i.LabTestCode == lisCode, ct);

            // We'll try to resolve LabTestId from catalog for FK safety
            long resolvedLabTestId = 0;
            string? resolvedLabTestName = null;

            // Try to resolve from existing item first
            if (item is not null && item.LabTestId > 0)
            {
                resolvedLabTestId = item.LabTestId;
                resolvedLabTestName = item.LabTestName;
            }
            else
            {
                // Resolve from LabTests by code
                var testRow = await _db.LabTests
                    .Where(t => !t.IsDeleted && t.Code == lisCode)
                    .Select(t => new { t.LabTestId, t.Name })
                    .FirstOrDefaultAsync(ct);

                if (testRow is not null)
                {
                    resolvedLabTestId = testRow.LabTestId;
                    resolvedLabTestName = testRow.Name;
                }
            }

            if (item is null)
            {
                if (resolvedLabTestId <= 0)
                {
                    _log.LogError("UpsertResult: No LabRequestItem and no LabTest found for code {code}. Acc={acc}. Aborting to avoid FK crash.",
                        lisCode, accession);
                    return;
                }

                // Create a placeholder request item (dev-friendly)
                item = new myLabRequestItem
                {
                    LabRequestId = sample.LabRequestId,
                    LabTestId = resolvedLabTestId,                  // ✅ set FK on item
                    LabTestCode = lisCode,
                    LabTestName = resolvedLabTestName ?? lisCode,
                    LabTestUnit = units ?? string.Empty,
                    LabTestPrice = 0m
                };
                _db.LabRequestItems.Add(item);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                // Make sure the existing item has a valid LabTestId
                if (item.LabTestId <= 0)
                {
                    if (resolvedLabTestId <= 0)
                    {
                        _log.LogError("UpsertResult: Existing item has no LabTestId and catalog lookup failed. Code {code}, Acc={acc}.",
                            lisCode, accession);
                        return;
                    }

                    item.LabTestId = resolvedLabTestId;                // ✅ repair item FK
                    if (string.IsNullOrWhiteSpace(item.LabTestName) && !string.IsNullOrWhiteSpace(resolvedLabTestName))
                        item.LabTestName = resolvedLabTestName;

                    await _db.SaveChangesAsync(ct);
                }
                else
                {
                    // Keep for use in result insert/update
                    resolvedLabTestId = item.LabTestId;
                    resolvedLabTestName = string.IsNullOrWhiteSpace(item.LabTestName) ? resolvedLabTestName : item.LabTestName;
                }
            }

            if (resolvedLabTestId <= 0)
            {
                _log.LogError("UpsertResult: Could not resolve LabTestId for code {code}. Acc={acc}.", lisCode, accession);
                return;
            }

            // 4) Upsert a single active result row for that item
            var now = DateTime.UtcNow;

            var result = await _db.LabResults
                .Where(r => r.LabRequestId == sample.LabRequestId && r.LabRequestItemId == item.LabRequestItemId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (result is null)
            {
                result = new myLabResult
                {
                    LabRequestId = sample.LabRequestId,
                    LabRequestItemId = item.LabRequestItemId,
                    LabTestId = resolvedLabTestId,                 // ✅ FK SAFE
                    LabTestCode = lisCode,
                    LabTestName = resolvedLabTestName ?? item.LabTestName ?? lisCode,

                    AccessionNumber = sample.AccessionNumber,
                    DeviceId = deviceId,
                    InstrumentTestCode = instrumentTestCode,

                    Value = value ?? string.Empty,
                    Unit = units ?? item.LabTestUnit ?? string.Empty,
                    Flag = flag,
                    RawFlag = rawFlagOrNotes,
                    Status = LabResultStatus.Final,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                _db.LabResults.Add(result);
            }
            else
            {
                result.DeviceId = deviceId;
                result.InstrumentTestCode = instrumentTestCode;

                result.LabTestId = resolvedLabTestId;              // ✅ ensure FK remains valid
                result.LabTestCode = lisCode;
                result.LabTestName = resolvedLabTestName ?? result.LabTestName ?? lisCode;

                result.Value = value ?? result.Value;
                result.Unit = units ?? result.Unit;
                result.Flag = flag ?? result.Flag;
                result.RawFlag = rawFlagOrNotes ?? result.RawFlag;
                result.Status = LabResultStatus.Final;
                result.UpdatedAt = now;
            }

            // 5) Optionally bump request status
            var req = await _db.LabRequests.FindAsync(new object[] { sample.LabRequestId }, ct);
            if (req is not null)
            {
                // choose your policy; here we set to InProgress if created, else keep existing
                if (req.Status == LabRequestStatus.Requested)
                    req.Status = LabRequestStatus.InProgress;
                req.UpdatedAt = now;
            }

            await _db.SaveChangesAsync(ct);
        }

    }
}

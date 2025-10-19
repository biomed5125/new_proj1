// HMS.Api/Features/Communication/CommBenchListEndpoints.cs
using HMS.Communication.Abstractions;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Infrastructure.Repositories; // CommBenchService
using HMS.Communication.Persistence;
using HMS.Realtime.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HMS.Api.Features.Communication.Bench;

public static class CommBenchListEndpoints
{
    // DTO
    public sealed class Row
    {
        public long Id { get; set; }
        public DateTime At { get; set; }
        public long DeviceId { get; set; }
        public string DeviceCode { get; set; } = "DEV";
        public string DeviceName { get; set; } = "";
        public string? PortName { get; set; }
        public string? ProfileName { get; set; }

        public string Direction { get; set; } = "";
        public string? Type { get; set; }
        public string? AccessionNumber { get; set; }
        public string? InstrumentTestCode { get; set; }
        public bool ChecksumOk { get; set; }
        public int Size { get; set; }
        public string? BusinessNo { get; set; }
    }
    public sealed record DeviceUpsertDto(
    string DeviceCode,
    string Name,
    string PortName,
    long AnalyzerProfileId,
    bool IsActive = true);

    public sealed record DeviceUpdateDto(
        string? Name,
        string? PortName,
        long? AnalyzerProfileId,
        bool? IsActive);


    public static IEndpointRouteBuilder MapCommBenchLists(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/comm/bench").WithTags("CommBench");

        // ---------------------------------------------------------------------
        // Devices
        // ---------------------------------------------------------------------
        g.MapGet("/devices", async (CommunicationDbContext db, CancellationToken ct) =>
        {
            var rows = await db.CommDevices.AsNoTracking()
                .Include(d => d.AnalyzerProfile)
                .OrderBy(d => d.DeviceCode)
                .Select(d => new
                {
                    id = d.CommDeviceId,          // 👈 simple names
                    code = d.DeviceCode,
                    name = d.Name,
                    port = d.PortName,
                    profile = d.AnalyzerProfile.Name
                })
                .ToListAsync(ct);

            return Results.Ok(rows);
        });

        // ---------------------------------------------------------------------
        // Inbound (RX)
        // GET /api/comm/bench/inbound?afterId=123&take=200
        // ---------------------------------------------------------------------
        g.MapGet("/inbound", async (
            CommunicationDbContext db,
            [FromQuery] long? afterId,
            [FromQuery] int take = 200,
            CancellationToken ct = default) =>
        {
            var q = db.Inbound.AsNoTracking();

            // paging pattern: if afterId present -> ascending from that id; else latest descending
            if (afterId.HasValue)
            {
                q = q.Where(x => x.CommMessageInboundId > afterId.Value)
                     .OrderBy(x => x.CommMessageInboundId);
            }
            else
            {
                q = q.OrderByDescending(x => x.CommMessageInboundId);
            }

            var list = await q
                .Join(db.CommDevices.AsNoTracking().Include(d => d.AnalyzerProfile),
                      i => i.DeviceId,
                      d => d.CommDeviceId,
                      (i, d) => new
                      {
                          i.CommMessageInboundId,
                          i.At,
                          i.DeviceId,
                          d.DeviceCode,
                          d.Name,
                          d.PortName,
                          ProfileName = d.AnalyzerProfile.Name,
                          i.Ascii,
                          i.Bytes
                      })
                .Take(take)
                .ToListAsync(ct);

            var rows = list.Select(x => new Row
            {
                Id = x.CommMessageInboundId,
                At = x.At,
                DeviceId = x.DeviceId,
                DeviceCode = x.DeviceCode,
                DeviceName = x.Name,
                PortName = x.PortName,
                ProfileName = x.ProfileName,
                Direction = "RX",
                Type = FirstRecordType(x.Ascii),
                AccessionNumber = TryAccession(x.Ascii),
                InstrumentTestCode = TryInstrCode(x.Ascii),
                ChecksumOk = true, // hook your stored flag here if you add it
                Size = x.Bytes != null ? x.Bytes.Length : x.Ascii?.Length ?? 0,
                BusinessNo = null
            }).ToList();

            return Results.Ok(rows);
        });

        // ---------------------------------------------------------------------
        // Outbound (TX)
        // GET /api/comm/bench/outbound?afterId=123&take=200
        // ---------------------------------------------------------------------
        g.MapGet("/outbound", async (
            CommunicationDbContext db,
            [FromQuery] long? afterId,
            [FromQuery] int take = 200,
            CancellationToken ct = default) =>
        {
            var q = db.Outbound.AsNoTracking();

            if (afterId.HasValue)
            {
                q = q.Where(x => x.CommMessageOutboundId > afterId.Value)
                     .OrderBy(x => x.CommMessageOutboundId);
            }
            else
            {
                q = q.OrderByDescending(x => x.CommMessageOutboundId);
            }

            var list = await q
                .Join(db.CommDevices.AsNoTracking().Include(d => d.AnalyzerProfile),
                      o => o.DeviceId,
                      d => d.CommDeviceId,
                      (o, d) => new
                      {
                          o.CommMessageOutboundId,
                          o.At,
                          o.DeviceId,
                          d.DeviceCode,
                          d.Name,
                          d.PortName,
                          ProfileName = d.AnalyzerProfile.Name,
                          o.Payload
                      })
                .Take(take)
                .ToListAsync(ct);

            var rows = list.Select(x => new Row
            {
                Id = x.CommMessageOutboundId,
                At = x.At,
                DeviceId = x.DeviceId,
                DeviceCode = x.DeviceCode,
                DeviceName = x.Name,
                PortName = x.PortName,
                ProfileName = x.ProfileName,
                Direction = "TX",
                Type = "Status",
                ChecksumOk = true,
                Size = x.Payload?.Length ?? 0
            }).ToList();

            return Results.Ok(rows);
        });

        // ---------------------------------------------------------------------
        // Send a simple test frame via the proper analyzer driver
        // POST /api/comm/bench/send-simple
        // body: { deviceId, accession, testCode, value, unit }
        // ---------------------------------------------------------------------
        // using Microsoft.Extensions.Configuration;
        // using Microsoft.EntityFrameworkCore;
        // using System.Text;

        g.MapPost("/send-simple", async (
            [FromBody] SendSimpleDto dto,
            CommunicationDbContext db,
            CommBenchService bench,
            IConfiguration cfg,
            CancellationToken ct) =>
        {
            // --- Validate -------------------------------------------------------------
            if (dto is null || dto.DeviceId <= 0) return Results.BadRequest("deviceId required.");
            if (string.IsNullOrWhiteSpace(dto.Accession)) return Results.BadRequest("accession required.");
            if (string.IsNullOrWhiteSpace(dto.TestCode)) return Results.BadRequest("testCode required.");

            var dev = await db.CommDevices.AsNoTracking()
                .FirstOrDefaultAsync(d => d.CommDeviceId == dto.DeviceId, ct);
            if (dev is null) return Results.NotFound("Device not found.");

            // --- Build ASTM/ASCII bytes via the selected analyzer driver --------------
            var bytes = await bench.BuildTestFrameAsync(
                dto.DeviceId,
                dto.Accession.Trim(),
                dto.TestCode.Trim().ToUpperInvariant(),
                (dto.Value ?? "0").Trim(),
                (dto.Unit ?? "").Trim(),
                ct);

            // --- Resolve input folder from config -------------------------------------
            // Prefer Communication:Transport:InputFolder (folder) OR
            // if only InputPath is supplied and points to a file, use its directory.
            var tSec = cfg.GetSection("Communication:Transport");
            var inputFolder = tSec["InputFolder"];
            var inputPath = tSec["InputPath"];

            string targetFolder;

            if (!string.IsNullOrWhiteSpace(inputFolder))
            {
                targetFolder = inputFolder!;
            }
            else if (!string.IsNullOrWhiteSpace(inputPath))
            {
                // If InputPath looks like a file path, use its directory; if it is already a directory, use it as-is.
                if (Path.HasExtension(inputPath))
                    targetFolder = Path.GetDirectoryName(inputPath)!;
                else
                    targetFolder = inputPath!;
            }
            else
            {
                // Final fallback: local comm_in under app folder
                targetFolder = Path.Combine(AppContext.BaseDirectory, "comm_in");
            }

            Directory.CreateDirectory(targetFolder);

            // Optional: read Pattern for consistency (default *.txt)
            var pattern = tSec["Pattern"];
            var ext = !string.IsNullOrWhiteSpace(pattern) && pattern!.Contains(".")
                ? Path.GetExtension(pattern)
                : ".txt";

            // --- Write file the worker will ingest ------------------------------------
            var safeCode = string.IsNullOrWhiteSpace(dev.DeviceCode) ? "DEV" : dev.DeviceCode.Trim();
            var fileName = $"{safeCode}_{dto.Accession.Trim()}_{dto.TestCode.Trim().ToUpperInvariant()}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
            var fullPath = Path.Combine(targetFolder, fileName);

            await System.IO.File.WriteAllBytesAsync(fullPath, bytes, ct);

            return Results.Ok(new { file = fullPath, size = bytes.Length });
        });


        // ---------------------------------------------------------------------
        // Profiles: list analyzer profiles for dropdowns
        // ---------------------------------------------------------------------
        g.MapGet("/profiles", async (CommunicationDbContext db, CancellationToken ct) =>
        {
            var rows = await db.AnalyzerProfiles.AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new {
                    p.AnalyzerProfileId,
                    p.Name,
                    p.Protocol,
                    p.DriverClass,
                    p.DefaultMode
                })
                .ToListAsync(ct);

            return Results.Ok(rows);
        });

        // ---------------------------------------------------------------------
        // Devices: create (optional, convenient from UI)
        // POST /api/comm/bench/devices
        // ---------------------------------------------------------------------
        g.MapPost("/devices", async (
            [FromBody] DeviceUpsertDto dto,
            CommunicationDbContext db,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(dto.DeviceCode) ||
                string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.PortName) ||
                dto.AnalyzerProfileId <= 0)
                return Results.BadRequest("deviceCode, name, portName and analyzerProfileId are required.");

            var exists = await db.CommDevices.AnyAsync(d => d.DeviceCode == dto.DeviceCode, ct);
            if (exists) return Results.Conflict($"DeviceCode already exists: {dto.DeviceCode}");

            var profExists = await db.AnalyzerProfiles.AnyAsync(p => p.AnalyzerProfileId == dto.AnalyzerProfileId, ct);
            if (!profExists) return Results.BadRequest("AnalyzerProfileId not found.");

            var row = new HMS.Communication.Infrastructure.Persistence.Entities.CommDevice
            {
                DeviceCode = dto.DeviceCode.Trim(),
                Name = dto.Name.Trim(),
                PortName = dto.PortName.Trim(),
                AnalyzerProfileId = dto.AnalyzerProfileId,
                IsActive = dto.IsActive
            };
            db.CommDevices.Add(row);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/comm/bench/devices/{row.CommDeviceId}", new
            {
                row.CommDeviceId,
                row.DeviceCode,
                row.Name,
                row.PortName,
                row.AnalyzerProfileId,
                row.IsActive
            });
        });

        // ---------------------------------------------------------------------
        // Devices: update basic fields
        // PUT /api/comm/bench/devices/{id}
        // ---------------------------------------------------------------------
        g.MapPut("/devices/{id:long}", async (
            long id,
            [FromBody] DeviceUpdateDto dto,
            CommunicationDbContext db,
            CancellationToken ct) =>
        {
            var d = await db.CommDevices.FirstOrDefaultAsync(x => x.CommDeviceId == id, ct);
            if (d is null) return Results.NotFound();

            if (dto.AnalyzerProfileId.HasValue)
            {
                var ok = await db.AnalyzerProfiles.AnyAsync(p => p.AnalyzerProfileId == dto.AnalyzerProfileId.Value, ct);
                if (!ok) return Results.BadRequest("AnalyzerProfileId not found.");
                d.AnalyzerProfileId = dto.AnalyzerProfileId.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name)) d.Name = dto.Name.Trim();
            if (!string.IsNullOrWhiteSpace(dto.PortName)) d.PortName = dto.PortName.Trim();
            if (dto.IsActive.HasValue) d.IsActive = dto.IsActive.Value;

            await db.SaveChangesAsync(ct);

            return Results.Ok(new
            {
                d.CommDeviceId,
                d.DeviceCode,
                d.Name,
                d.PortName,
                d.AnalyzerProfileId,
                d.IsActive
            });
        });

        // ---------------------------------------------------------------------
        // Devices: get one (useful for edit forms)
        // GET /api/comm/bench/devices/{id}
        // ---------------------------------------------------------------------
        g.MapGet("/devices/{id:long}", async (long id, CommunicationDbContext db, CancellationToken ct) =>
        {
            var d = await db.CommDevices.AsNoTracking()
                .Include(x => x.AnalyzerProfile)
                .FirstOrDefaultAsync(x => x.CommDeviceId == id, ct);

            if (d is null) return Results.NotFound();

            return Results.Ok(new
            {
                d.CommDeviceId,
                d.DeviceCode,
                d.Name,
                d.PortName,
                d.IsActive,
                d.AnalyzerProfileId,
                AnalyzerProfile = d.AnalyzerProfile.Name
            });
        });

        // Optional: quick manual test
        app.MapPost("/api/lab/diag/broadcast-demo", async (ILabRealtime rt) =>
        {
            await rt.BroadcastAsync(new HubEventDto(
                At: DateTimeOffset.UtcNow,
                DeviceId: 2025900000001,
                Kind: "ResultPosted",
                Accession: "DEMO-123",
                InstrumentCode: "GLU",
                Value: "155",
                Units: "mg/dL",
                Flag: "N"
            ));
            return Results.Ok();
        });

        app.MapGet("/api/lab/live/recent", async (CommunicationDbContext db) =>
        {
            var rows = await db.Events
                .OrderByDescending(e => e.At)
                .Take(50)
                .Select(e => new {
                    e.At,
                    e.DeviceId,
                    e.Kind,
                    e.Accession,
                    e.InstrumentCode,
                    e.Value,
                    e.Units,
                    e.Flag
                })
                .ToListAsync();

            return Results.Ok(rows);
        });

        app.MapGet("/api/comm/diag", (
        IConfiguration cfg,
        IDbContextFactory<CommunicationDbContext> commFactory,
        IEnumerable<IFrameTracer> tracers
        ) => Results.Ok(new
            {
                InputPath = cfg["Communication:Transport:InputPath"],
                ArchiveFolder = cfg["Communication:Transport:ArchiveFolder"],
                Tracers = tracers.Select(t => t.GetType().Name).ToArray(),
                CanOpenDb = Try(() => {
                using var _ = commFactory.CreateDbContext();
                return true;
            })
        }));

        static T Try<T>(Func<T> f) { try { return f(); } catch { return default!; } }

        app.MapPost("/api/comm/diag/trace-test", async (IFrameTracer tracer) =>
        {
            var frame = new RawFrame(
                new DeviceRef(2025900000001, "ROCHE1"),
                DateTimeOffset.UtcNow,
                FrameDirection.Rx,
                "Diag",
                Encoding.ASCII.GetBytes("R|1|^^^GLU^1|123|mg/dL|N||F\r"),
                "R|1|^^^GLU^1|123|mg/dL|N||F\r"
            );
            await tracer.TraceAsync(frame, CancellationToken.None);
            return Results.Ok(new { ok = true, at = frame.At });
        });

        return app;
    }

    public sealed record SendSimpleDto(long DeviceId, string Accession, string TestCode, string? Value, string? Unit);

    // ---------------- in-memory helpers (NOT translated by EF) ----------------
    private static string? FirstRecordType(string? ascii)
    {
        if (string.IsNullOrEmpty(ascii)) return null;
        foreach (var line in ascii.Split('\r', StringSplitOptions.RemoveEmptyEntries))
            if (line.Length > 1) return line[0] + "|";
        return null;
    }

    private static string? TryAccession(string? ascii)
    {
        if (string.IsNullOrEmpty(ascii)) return null;
        foreach (var line in ascii.Split('\r', StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith("O|"))
            {
                var f = line.Split('|');
                if (f.Length > 2) return string.IsNullOrWhiteSpace(f[2]) ? null : f[2];
            }
        }
        return null;
    }

    private static string? TryInstrCode(string? ascii)
    {
        if (string.IsNullOrEmpty(ascii)) return null;
        foreach (var line in ascii.Split('\r', StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith("R|"))
            {
                var f = line.Split('|');
                if (f.Length > 2)
                {
                    var comp = f[2].Split('^');
                    if (comp.Length >= 4) return string.IsNullOrWhiteSpace(comp[3]) ? null : comp[3];
                }
            }
        }
        return null;
    }
}

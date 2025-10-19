//using HMS.Communication.Infrastructure.Persistence;
//using HMS.Communication.Persistence;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using static HMS.Api.Features.Communication.Bench.CommBenchListEndpoints;

//namespace HMS.Api.Features.Communication.Bench
//{
//    // Request DTO (names match your CommBench client)
//    //public sealed record SendSimpleDto(long DeviceId, string Accession, string TestCode, string? Value, string? Unit);

//    public static class CommBenchSendSimpleEndpoints
//    {
//        public static IEndpointRouteBuilder MapCommBenchSendSimple(this IEndpointRouteBuilder app)
//        {
//            var grp = app.MapGroup("/api/comm/bench").WithTags("CommBench");

//            grp.MapPost("/send-simple", async (
//                SendSimpleDto dto,
//                CommunicationDbContext db,
//                IConfiguration cfg,
//                CancellationToken ct) =>
//            {
//                if (dto.DeviceId <= 0) return Results.BadRequest("deviceId required.");
//                if (string.IsNullOrWhiteSpace(dto.Accession)) return Results.BadRequest("accession required.");
//                if (string.IsNullOrWhiteSpace(dto.TestCode)) return Results.BadRequest("testCode required.");

//                var dev = await db.CommDevices
//                    .AsNoTracking()
//                    .FirstOrDefaultAsync(d => d.CommDeviceId == dto.DeviceId, ct);

//                if (dev is null) return Results.NotFound("Device not found.");

//                var value = string.IsNullOrWhiteSpace(dto.Value) ? "0" : dto.Value!;
//                var unit = dto.Unit ?? string.Empty;

//                // Minimal ASTM demo frame (H,P,O,R,L), CR-terminated lines
//                var now = DateTime.UtcNow;
//                string[] lines =
//                {
//                    $"H|\\^&|||{dev.DeviceCode}^{dev.Name}|||||{now:yyyyMMdd} {now:HHmmss}L",
//                    "P|1",
//                    $"O|1|{dto.Accession}||^^^{dto.TestCode}|R|N||||||{now:yyyyMMddHHmmss}",
//                    $"R|1|^^^{dto.TestCode}|{value}|{unit}||||N|F|{now:yyyyMMddHHmmss}",
//                    "L|1|N"
//                };
//                var frame = string.Join("\r", lines) + "\r";

//                // Where to drop: Communication:Transport:InputPath (fallback: /bin/.../comm_in)
//                var inDir = cfg.GetValue<string>("Communication:Transport:InputPath")
//                            ?? Path.Combine(AppContext.BaseDirectory, "comm_in");
//                Directory.CreateDirectory(inDir);

//                var file = Path.Combine(
//                    inDir,
//                    $"{dev.DeviceCode}_{dto.Accession}_{dto.TestCode}_{DateTime.UtcNow:yyyyMMddHHmmss}.txt");

//                await File.WriteAllTextAsync(file, frame, ct);

//                return Results.Ok(new { file, size = frame.Length });
//            }).WithName("CommBench.SendSimpleRaw");

//            return app;
//        }
//    }
//}

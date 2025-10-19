//// File: HMS.Api/Features/Communication/CommBenchEndpoints.cs
//using HMS.Communication.Abstractions;
//using HMS.Communication.Infrastructure.Observability;
//using HMS.Module.Lab.Infrastructure.Persistence;
//using HMS.Module.Lab.Features.Lab.Models.Entities;
//using HMS.Module.Lab.Service;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Api.Features.Communication;

//public static class CommBenchEndpoints
//{
//    public sealed record CreateOrderDto(long PatientId, List<string> TestCodes, string Priority = "Routine");
//    public sealed record OrderResponse(long LabRequestId, string OrderNo, long LabSampleId, string Accession);

//    public sealed record PushFramesDto(string DeviceCode, List<string> Frames);
//    public sealed record PushOk(string Path, int Count);

//    public static IEndpointRouteBuilder MapCommBenchEndpoints(this IEndpointRouteBuilder app)
//    {
//        var g = app.MapGroup("/api/comm/bench").WithTags("Communication Bench");

//        // 1) Create an order by test CODES and immediately COLLECT a sample (accession is generated)
//        g.MapPost("/order", async (
//            CreateOrderDto dto,
//            LabDbContext db,
//            ILabOrderService orders,
//            ILabSampleService samples,
//            CancellationToken ct) =>
//        {
//            if (dto.TestCodes is null || dto.TestCodes.Count == 0) return Results.BadRequest("No test codes.");

//            // map codes -> testIds
//            var testIds = await db.LabTests
//                .Where(t => dto.TestCodes.Contains(t.Code) && !t.IsDeleted)
//                .Select(t => t.LabTestId)
//                .ToListAsync(ct);
//            if (testIds.Count == 0) return Results.BadRequest("No valid test codes found.");

//            // create request
//            var reqId = await orders.CreateRequestAsync(
//                new HMS.Module.Lab.Features.Lab.Models.Dtos.CreateLabRequestDto(
//                    PatientId: dto.PatientId,
//                    AdmissionId: null,
//                    DoctorId: null,
//                    Priority: string.IsNullOrWhiteSpace(dto.Priority) ? "Routine" : dto.Priority,
//                    Notes: "comm-bench",
//                    TestIds: testIds,
//                    PanelIds: null
//                ),
//                ct);

//            // collect -> accession
//            var sampleId = await samples.CollectAsync(reqId, "comm-bench", ct);
//            var acc = await db.LabSamples.Where(s => s.LabSampleId == sampleId).Select(s => s.AccessionNumber).FirstAsync(ct);
//            var orderNo = await db.LabRequests.Where(r => r.LabRequestId == reqId).Select(r => r.OrderNo).FirstAsync(ct);

//            return Results.Ok(new OrderResponse(reqId, orderNo, sampleId, acc));
//        });

//        // 2) Push raw frames (with <STX>/<ETX>/<CR>/<LF> tokens) into the file feed the host tails
//        g.MapPost("/push", async (
//            PushFramesDto dto,
//            IConfiguration cfg,
//            CancellationToken ct) =>
//        {
//            // Resolve file path the FileFeed host is tailing:
//            var path = cfg["Communication:Transport:Path"]
//                       ?? cfg["Communication:Transport:InFolder"]
//                       ?? "bench/astm-feed.txt";

//            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

//            // Append frames as bytes
//            await using var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
//            foreach (var f in dto.Frames)
//            {
//                var bytes = TokenToBytes(f);
//                await fs.WriteAsync(bytes.AsMemory(), ct);
//            }
//            await fs.FlushAsync(ct);

//            return Results.Ok(new PushOk(path, dto.Frames.Count));
//        });

//        // 3) Simple health
//        g.MapGet("/health", (IConfiguration cfg) =>
//        {
//            var path = cfg["Communication:Transport:Path"]
//                       ?? cfg["Communication:Transport:InFolder"]
//                       ?? "bench/astm-feed.txt";
//            return Results.Ok(new { feed = path, now = DateTime.UtcNow });
//        });

//        return app;
//    }

//    // --- helpers ---
//    private static byte[] TokenToBytes(string s)
//    {
//        // Replace <STX>/<ETX>/<CR>/<LF> style tokens with control chars.
//        // Also normalize common slip-ups like L|1<N to L|1|N
//        var fixedText = s.Replace("<STX>", "\x02")
//                         .Replace("<ETX>", "\x03")
//                         .Replace("<ETB>", "\x17")
//                         .Replace("<EOT>", "\x04")
//                         .Replace("<ENQ>", "\x05")
//                         .Replace("<ACK>", "\x06")
//                         .Replace("<NAK>", "\x15")
//                         .Replace("<CR>", "\r")
//                         .Replace("<LF>", "\n")
//                         .Replace("L|1<N", "L|1|N"); // common typo in samples

//        return System.Text.Encoding.ASCII.GetBytes(fixedText);
//    }
//}

using global::HMS.Communication.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace HMS.Api.Features.Communication.Bench
{
    public static class CommBenchPushEndpoints
    {
        public sealed record PushRequest(string DeviceCode, List<string> Frames);

        // token replacer: "<STX>" → \x02, "<ETX>" → \x03, "<CR>" → \r, "<LF>" → \n
        private static string ExpandTokens(string s) => s
            .Replace("<STX>", "\x02")
            .Replace("<ETX>", "\x03")
            .Replace("<CR>", "\r")
            .Replace("<LF>", "\n");

        
        public static IEndpointRouteBuilder MapCommBenchPush(this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/api/comm/bench").WithTags("CommBench");

            g.MapPost("/push", async (
                [FromBody] PushRequest req,
                IConfiguration cfg,
                CancellationToken ct) =>
            {
                // Resolve the folder we agreed on
                var tSec = cfg.GetSection("Communication:Transport");
                var inputFolder = tSec["InputFolder"];
                if (string.IsNullOrWhiteSpace(inputFolder))
                    return Results.BadRequest("Communication:Transport:InputFolder is not configured.");

                Directory.CreateDirectory(inputFolder);

                var pattern = tSec["Pattern"];
                var ext = !string.IsNullOrWhiteSpace(pattern) && Path.GetExtension(pattern) is { Length: > 0 } e ? e : ".txt";

                var safeCode = string.IsNullOrWhiteSpace(req.DeviceCode) ? "DEV" : req.DeviceCode.Trim();
                var fileName = $"{safeCode}_PUSH_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                var fullPath = Path.Combine(inputFolder, fileName);

                // Write one complete frame file the host will ingest
                await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                await using var sw = new StreamWriter(fs, Encoding.ASCII) { NewLine = "\r\n" };

                foreach (var raw in req.Frames)
                {
                    var line = ExpandTokens(raw);
                    if (!line.EndsWith("\r\n", StringComparison.Ordinal))
                        line += "\r\n";
                    await sw.WriteAsync(line);
                }

                await sw.FlushAsync();
                return Results.Ok(new { file = fullPath });
            });

            return app;
        }
    }
}
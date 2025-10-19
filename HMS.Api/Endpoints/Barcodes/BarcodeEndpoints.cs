using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing.Imaging;
using ZXing.QrCode.Internal;
using System.Drawing;

namespace HMS.Api.Endpoints.Barcodes;

public static class BarcodeEndpoints
{
    // Use a unique DTO name and force FromBody to avoid binder confusion
    public sealed record BarcodeIssueRequest(long PatientId, string[] TestCodes);
    public sealed record BarcodeIssueResponse(
        long LabRequestId, string OrderNo,
        long LabSampleId, string AccessionNumber);

    public static IEndpointRouteBuilder MapBarcodeEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/barcodes").WithTags("Barcodes");

        // simple helper to eyeball the next accession
        g.MapGet("/next", (string? prefix) =>
        {
            var acc = BuildAccession(prefix);
            return Results.Ok(new { accession = acc });
        });

        g.MapPost("/issue", async (
            [FromBody] BarcodeIssueRequest req,
            LabDbContext db,
            CancellationToken ct) =>
        {
            if (req.TestCodes is null || req.TestCodes.Length == 0)
                return Results.BadRequest("testCodes is required.");

            var codes = req.TestCodes
                .Select(c => c.Trim().ToUpperInvariant())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToArray();

            if (codes.Length == 0)
                return Results.BadRequest("testCodes is empty.");

            // Map codes -> tests
            var tests = await db.LabTests.AsNoTracking()
                .Where(t => codes.Contains(t.Code))
                .Select(t => new { t.LabTestId, t.Code })
                .ToListAsync(ct);

            var missing = codes.Except(tests.Select(t => t.Code), StringComparer.OrdinalIgnoreCase).ToArray();
            if (missing.Length > 0)
                return Results.BadRequest($"Unknown test codes: {string.Join(", ", missing)}");

            var now = DateTime.UtcNow;
            var orderNo = $"ORD-{now:yyyyMMddHHmmss}";
            var request = new myLabRequest
            {
                OrderNo = orderNo,
                PatientId = req.PatientId,
                Status = LabRequestStatus.Requested,
                CreatedAt = now,
                CreatedBy = "api"
            };
            db.LabRequests.Add(request);
            await db.SaveChangesAsync(ct); // get LabRequestId

            foreach (var t in tests)
            {
                db.LabRequestItems.Add(new myLabRequestItem
                {
                    LabRequestId = request.LabRequestId,
                    LabTestId = t.LabTestId,
                    LabTestCode = t.Code,          // <- add this line
                    CreatedAt = now,
                    CreatedBy = "api"
                });
            }
            await db.SaveChangesAsync(ct);

            var accession = BuildAccession("ACC");
            var sample = new myLabSample
            {
                LabRequestId = request.LabRequestId,
                AccessionNumber = accession,
                Status = LabSampleStatus.Collected,
                CreatedAt = now,
                CreatedBy = "api"
            };
            db.LabSamples.Add(sample);
            await db.SaveChangesAsync(ct);

            return Results.Ok(new BarcodeIssueResponse(
                request.LabRequestId, orderNo, sample.LabSampleId, accession));
        });

        return app;
    }

    public static IEndpointRouteBuilder MapBarcodeScanEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/barcodes/scan/{accession}", async (
            string accession,
            LabDbContext db,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(accession)) return Results.BadRequest("accession is required.");

            var sample = await db.LabSamples
                .FirstOrDefaultAsync(s => s.AccessionNumber == accession && !s.IsDeleted, ct);

            // Always log the scan event
            db.BarcodeEvents.Add(new myBarcodeEvent
            {
                AccessionNumber = accession,
                Event = "SCANNED",
                At = DateTimeOffset.UtcNow,
                Who = "comm-bench",
                Note = sample is null ? "sample not found" : null
            });

            if (sample is null)
            {
                await db.SaveChangesAsync(ct);
                return Results.NotFound(new { message = "Sample not found for accession.", accession });
            }

            // Optional: mark as received on scan
            if (sample.Status == LabSampleStatus.Collected)
            {
                sample.Status = LabSampleStatus.Received;
                sample.ReceivedAtUtc = DateTime.UtcNow;
                sample.ReceivedBy = "comm-bench";
            }

            await db.SaveChangesAsync(ct);
            return Results.Ok(new { sample.LabSampleId, sample.LabRequestId, accession, status = sample.Status.ToString() });
        })
        .WithTags("Barcodes");

        return app;
    }

    private static string BuildAccession(string? prefix)
    {
        var p = string.IsNullOrWhiteSpace(prefix) ? "ACC" : prefix.Trim();
        return $"{p}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
    }
    // using QRCoder;
    public static IEndpointRouteBuilder MapBarcodeImageEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/barcodes");

        // your Code128 stays as-is if it already returns byte[]
        g.MapGet("/code128/{text}", (string text) =>
        {
            var png = Code128Render.RenderPng(text, textBelow: true, height: 60, scale: 2);
            return Results.File(png, "image/png");
        });

        // ✅ QR as PNG bytes (no System.Drawing)
        g.MapGet("/qr/{text}", (string text) =>
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            var qrPng = new PngByteQRCode(data);   // or new PngByteQRCode(data, SKManaged) if you later use Skia
            var bytes = qrPng.GetGraphic(pixelsPerModule: 10); // 10 = size

            return Results.File(bytes, "image/png");
        });

        return app;
    }

}
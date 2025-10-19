using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Features.Lab.Service; // if your services live elsewhere, keep your original using
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints
{
    public static class ResultEndpoints
    {
        public static IEndpointRouteBuilder MapLabResultEndpoints(this IEndpointRouteBuilder app)
        {
            // Group is already /api/v1/lab/results
            var g = app.MapGroup("/api/v1/lab/results").WithTags("Laboratory");

            // A) WORKLIST   -> /api/v1/lab/results/worklist?acc=ACC-...
            g.MapGet("/worklist", async (
                [FromQuery] string acc,
                LabDbContext db,
                CancellationToken ct) =>
            {
                var reqId = await db.LabSamples
                    .AsNoTracking()
                    .Where(s => s.AccessionNumber == acc)
                    .Select(s => s.LabRequestId)
                    .SingleOrDefaultAsync(ct);

                if (reqId == 0) return Results.NotFound("Unknown accession.");

                var q =
                    from i in db.LabRequestItems.AsNoTracking()
                    where i.LabRequestId == reqId
                    select new
                    {
                        i.LabTestId,
                        code = i.LabTest.Code,
                        name = i.LabTest.Name,
                        unit = i.LabTest.Unit,
                        refLow = i.LabTest.RefLow,
                        refHigh = i.LabTest.RefHigh,
                        currentValue = db.LabResults
                            .Where(r => r.AccessionNumber == acc && r.LabTestId == i.LabTestId)
                            .OrderByDescending(r => r.CreatedAt)
                            .Select(r => r.Value)
                            .FirstOrDefault(),
                        flag = db.LabResults
                            .Where(r => r.AccessionNumber == acc && r.LabTestId == i.LabTestId)
                            .OrderByDescending(r => r.CreatedAt)
                            .Select(r => r.Flag)
                            .FirstOrDefault()
                    };

                var rows = await q.OrderBy(x => x.code).ToListAsync(ct);
                return Results.Ok(rows);
            });

            // B) FINALIZE   -> /api/v1/lab/results/finalize?acc=ACC-...
            g.MapPost("/finalize", async (
                [FromQuery] string acc,
                LabDbContext db,
                CancellationToken ct) =>
            {
                var results = await db.LabResults
                    .Where(r => r.AccessionNumber == acc)
                    .ToListAsync(ct);

                if (!results.Any()) return Results.BadRequest("No results to finalize.");

                foreach (var r in results)
                {
                    r.Status = LabResultStatus.Final;
                    r.CreatedAt = DateTime.UtcNow;
                    r.CreatedBy = "ui";
                }
                await db.SaveChangesAsync(ct);
                return Results.Ok(new { status = "final" });
            });

            // (REMOVED) C) REPORT HEAD  -> now lives in ReportEndpoints
            // (REMOVED) D) REPORT ROWS  -> now lives in ReportEndpoints

            // E) BARCODE SCAN (optional location)
            g.MapPost("/barcodes/scan/{accession}", async (
                string accession,
                LabDbContext db,
                CancellationToken ct) =>
            {
                var s = await db.LabSamples.FirstOrDefaultAsync(x => x.AccessionNumber == accession, ct);
                if (s is null) return Results.NotFound();

                if (s.Status != LabSampleStatus.Received)
                {
                    s.Status = LabSampleStatus.Received;
                    s.ReceivedAtUtc = DateTime.UtcNow;
                    s.ReceivedBy = "scan";
                    await db.SaveChangesAsync(ct);
                }
                return Results.Ok(new { status = s.Status });
            });

            // Enter results
            g.MapPost("/enter", async ([FromBody] IEnumerable<EnterResultDto> lines,
                        [FromServices] IValidator<EnterResultDto> v,
                        [FromServices] ILabResultService svc,
                        CancellationToken ct) =>
            {
                foreach (var l in lines)
                {
                    var r = await v.ValidateAsync(l, ct);
                    if (!r.IsValid) return Results.ValidationProblem(r.ToDictionary());
                }
                await svc.EnterAsync(lines, ct);
                return Results.Ok();
            });

            // Approve results
            g.MapPost("/approve", async ([FromBody] ApproveResultsDto dto,
                [FromServices] IValidator<ApproveResultsDto> v,
                [FromServices] ILabResultService svc,
                CancellationToken ct) =>
            {
                var val = await v.ValidateAsync(dto, ct);
                if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());
                var ok = await svc.ApproveAsync(dto.LabRequestId, dto.ApprovedByDoctorId, ct);
                return ok ? Results.Ok() : Results.NotFound();
            });

            // Results by request id
            g.MapGet("/by-request/{labRequestId:long}", async (
                long labRequestId,
                [FromServices] LabDbContext db,
                CancellationToken ct) =>
            {
                var lines = await
                (
                    from r in db.LabResults.AsNoTracking()
                    join i in db.LabRequestItems.AsNoTracking() on r.LabRequestItemId equals i.LabRequestItemId
                    join t in db.LabTests.AsNoTracking() on i.LabTestId equals t.LabTestId
                    where r.LabRequestId == labRequestId && !r.IsDeleted
                    orderby t.Code
                    select new ResultLineDto(
                        i.LabRequestItemId,
                        t.Code,
                        t.Name,
                        r.Value,
                        r.Unit ?? t.Unit,
                        t.RefLow,
                        t.RefHigh,
                        r.Flag.HasValue ? r.Flag.Value.ToString() : string.Empty
                    )
                ).ToListAsync(ct);

                var orderNo = await db.LabRequests
                    .Where(x => x.LabRequestId == labRequestId)
                    .Select(x => x.OrderNo)
                    .FirstOrDefaultAsync(ct) ?? string.Empty;

                return Results.Ok(new ResultDetailDto(labRequestId, orderNo, lines));
            });

            // UPSERT by accession (instrument feed)
            g.MapPost("/upsert-by-accession", async (
                UpsertByAccessionDto dto,
                Service.ILabResultWriter writer,
                CancellationToken ct) =>
            {
                await writer.UpsertResultAsync(dto.Accession, dto.DeviceId, dto.InstrumentTestCode,
                                               dto.Value, dto.Unit, dto.Flag, dto.RawFlagOrNotes, ct);
                return Results.Ok();
            });

            return app;
        }
    }
}

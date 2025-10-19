using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;
public static class SampleEndpoints
{
    public static IEndpointRouteBuilder MapLabSampleEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/samples").WithTags("Laboratory");

        g.MapPost("/collect", async ([FromBody] CollectSampleDto dto,
            [FromServices] ILabSampleService svc, CancellationToken ct) =>
        {
            var id = await svc.CollectAsync(dto.LabRequestId, dto.Collector, ct);
            return Results.Ok(new { labSampleId = id });
        });

        g.MapPost("/receive", async ([FromBody] ReceiveSampleDto dto,
            [FromServices] ILabSampleService svc, CancellationToken ct) =>
        {
            var ok = await svc.ReceiveAsync(dto.LabSampleId, dto.Receiver, ct);
            return ok ? Results.Ok() : Results.NotFound();
        });

        g.MapGet("/by-request/{labRequestId:long}", async (long labRequestId, [FromServices] LabDbContext db, CancellationToken ct) =>
        {
            var s = await db.LabSamples.FirstOrDefaultAsync(x => x.LabRequestId == labRequestId, ct);
            return s is null ? Results.NotFound() : Results.Ok(s);
        });
        g.MapPost("/mark-printed", async ([FromQuery] string acc,[FromServices] LabDbContext db,CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(acc)) return Results.BadRequest("Missing accession.");
            var s = await db.LabSamples.FirstOrDefaultAsync(x => x.AccessionNumber == acc && !x.IsDeleted, ct);
            if (s is null) return Results.NotFound();

            // Idempotent
            s.LabelPrinted = true;
            if (s.Status == LabSampleStatus.Collected) s.Status = LabSampleStatus.Labeled;
            s.UpdatedAt = DateTime.UtcNow;
            s.UpdatedBy = "labels";
            await db.SaveChangesAsync(ct);

            return Results.Ok();
        });

        return app;
    }
}

using FluentValidation;

//using HMS.Module.Lab.Features.Lab.Infrastructure;

//using HMS.Communication.Infrastructure.Persistence.Repositories;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Validations;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;
public static class RequestEndpoints
{
    public static IEndpointRouteBuilder MapLabRequestEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/requests").WithTags("Laboratory");

        g.MapPost("/", async (
            [FromBody] CreateLabRequestDto dto,
            [FromServices] IValidator<CreateLabRequestDto> v,
            [FromServices] ILabOrderService svc,
            CancellationToken ct) =>
        {
            var val = await v.ValidateAsync(dto, ct);
            if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());
            var id = await svc.CreateRequestAsync(dto, ct);
            return Results.Created($"/api/v1/lab/requests/{id}", new { labRequestId = id });
        });

        g.MapGet("/{id:long}", async (long id, [FromServices] LabDbContext db, CancellationToken ct) =>
        {
            var req = await db.LabRequests.Include(r => r.Items).FirstOrDefaultAsync(r => r.LabRequestId == id, ct);
            return req is null ? Results.NotFound() : Results.Ok(req);
        });
        // Features/Lab/Endpoints/RequestEndpoints.cs (append)
        g.MapGet("/find/by-accession/{accession}", async (string accession, LabDbContext db, CancellationToken ct) =>
        {
            var reqId = await db.LabSamples.AsNoTracking()
                .Where(s => s.AccessionNumber == accession)
                .Select(s => s.LabRequestId)
                .FirstOrDefaultAsync(ct);

            if (reqId == 0) return Results.NotFound();
            var orderNo = await db.LabRequests.AsNoTracking()
                .Where(r => r.LabRequestId == reqId)
                .Select(r => r.OrderNo)
                .FirstOrDefaultAsync(ct) ?? string.Empty;

            return Results.Ok(new { labRequestId = reqId, orderNo });
        });

        g.MapGet("/find/by-order/{orderNo}", async (string orderNo, LabDbContext db, CancellationToken ct) =>
        {
            var req = await db.LabRequests.AsNoTracking()
                .Where(r => r.OrderNo == orderNo)
                .Select(r => new { r.LabRequestId, r.OrderNo })
                .FirstOrDefaultAsync(ct);

            return req is null ? Results.NotFound() : Results.Ok(req);
        });
        // Features/Lab/Endpoints/RequestEndpoints.cs (append)
        g.MapGet("/find/by-test/{labTestId:long}", async (long labTestId, LabDbContext db, CancellationToken ct) =>
        {
            var rows = await (
                from i in db.LabRequestItems.AsNoTracking()
                join r in db.LabRequests.AsNoTracking() on i.LabRequestId equals r.LabRequestId
                where i.LabTestId == labTestId && !i.IsDeleted && !r.IsDeleted
                orderby r.CreatedAt descending
                select new { r.LabRequestId, r.OrderNo, r.PatientId, r.CreatedAt }
            ).Take(100).ToListAsync(ct);

            return rows.Count == 0 ? Results.NotFound() : Results.Ok(rows);
        });

        // DELETE /api/v1/lab/requests/{id}
        g.MapDelete("/{labRequestId:long}", async (
            long labRequestId,
            LabDbContext db,
            CancellationToken ct) =>
        {
            var r = await db.LabRequests
                .Include(x => x.Items)
                .Include(x => x.Samples)
                .FirstOrDefaultAsync(x => x.LabRequestId == labRequestId, ct);

            if (r is null) return Results.NotFound();
            // Only allow delete if nothing finalized yet
            var hasFinal = await db.LabResults.AnyAsync(x => x.LabRequestId == labRequestId &&
                                                             x.Status == Models.Enums.LabResultStatus.Final, ct);
            if (hasFinal) return Results.BadRequest("Cannot delete: results are final.");

            r.IsDeleted = true;
            foreach (var i in r.Items) i.IsDeleted = true;
            foreach (var s in r.Samples) s.IsDeleted = true;

            await db.SaveChangesAsync(ct);
            return Results.Ok();
        });

        return app;
    }
}

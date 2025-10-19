using HMS.Module.Admission.Features.Admission.Models.Dtos;
using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Admission.Features.Admission.Endpoints;

public static class WardEndpoints
{
    public static IEndpointRouteBuilder MapWardEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/wards").WithTags("Wards v2");

        // LIST
        g.MapGet("/", async (AdmissionDbContext db, CancellationToken ct) =>
        {
            var items = await db.Wards.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.WardId)
                .Select(x => new WardDto(x.WardId, x.Name))
                .ToListAsync(ct);
            return Results.Ok(items);
        });

        // GET by id
        g.MapGet("/{id:long}", async (long id, AdmissionDbContext db, CancellationToken ct) =>
        {
            var x = await db.Wards.AsNoTracking()
                .FirstOrDefaultAsync(e => e.WardId == id && !e.IsDeleted, ct);
            return x is null ? Results.NotFound() : Results.Ok(new WardDto(x.WardId, x.Name));
        });

        // CREATE
        g.MapPost("/", async (WardDto dto, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = new myWard { Name = dto.Name, CreatedAt = DateTime.UtcNow, CreatedBy = "api" };
            db.Wards.Add(e);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v2/wards/{e.WardId}", new WardDto(e.WardId, e.Name));
        });

        // UPDATE
        g.MapPut("/{id:long}", async (long id, WardDto dto, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = await db.Wards.FirstOrDefaultAsync(x => x.WardId == id && !x.IsDeleted, ct);
            if (e is null) return Results.NotFound();
            e.Name = dto.Name;
            e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = "api";
            await db.SaveChangesAsync(ct);
            return Results.Ok(new WardDto(e.WardId, e.Name));
        });

        // DELETE (soft)
        g.MapDelete("/{id:long}", async (long id, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = await db.Wards.FirstOrDefaultAsync(x => x.WardId == id && !x.IsDeleted, ct);
            if (e is null) return Results.NotFound();
            e.IsDeleted = true; e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = "api";
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        return app;
    }
}

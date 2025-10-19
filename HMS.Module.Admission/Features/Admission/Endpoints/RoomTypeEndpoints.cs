using HMS.Module.Admission.Features.Admission.Models.Dtos;
using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Admission.Features.Admission.Endpoints;

public static class RoomTypeEndpoints
{
    public static IEndpointRouteBuilder MapRoomTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/roomtypes").WithTags("Room Types v2");

        // LIST
        g.MapGet("/", async (AdmissionDbContext db, CancellationToken ct) =>
        {
            var items = await db.RoomTypes.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.RoomTypeId)
                .Select(x => new RoomTypeDto(x.RoomTypeId, x.Name, x.DailyRate))
                .ToListAsync(ct);
            return Results.Ok(items);
        });

        // GET by id
        g.MapGet("/{id:long}", async (long id, AdmissionDbContext db, CancellationToken ct) =>
        {
            var x = await db.RoomTypes.AsNoTracking()
                .FirstOrDefaultAsync(e => e.RoomTypeId == id && !e.IsDeleted, ct);
            return x is null ? Results.NotFound()
                             : Results.Ok(new RoomTypeDto(x.RoomTypeId, x.Name, x.DailyRate));
        });

        // CREATE
        g.MapPost("/", async (RoomTypeDto dto, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = new myRoomType
            {
                Name = dto.Name,
                DailyRate = dto.DailyRate,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "api"
            };
            db.RoomTypes.Add(e);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v2/roomtypes/{e.RoomTypeId}",
                new RoomTypeDto(e.RoomTypeId, e.Name, e.DailyRate));
        });

        // UPDATE
        g.MapPut("/{id:long}", async (long id, RoomTypeDto dto, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = await db.RoomTypes.FirstOrDefaultAsync(x => x.RoomTypeId == id && !x.IsDeleted, ct);
            if (e is null) return Results.NotFound();
            e.Name = dto.Name;
            e.DailyRate = dto.DailyRate;
            e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = "api";
            await db.SaveChangesAsync(ct);
            return Results.Ok(new RoomTypeDto(e.RoomTypeId, e.Name, e.DailyRate));
        });

        // DELETE (soft)
        g.MapDelete("/{id:long}", async (long id, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = await db.RoomTypes.FirstOrDefaultAsync(x => x.RoomTypeId == id && !x.IsDeleted, ct);
            if (e is null) return Results.NotFound();
            e.IsDeleted = true; e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = "api";
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        return app;
    }
}

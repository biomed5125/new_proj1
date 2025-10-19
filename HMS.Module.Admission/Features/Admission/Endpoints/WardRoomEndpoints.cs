using HMS.Module.Admission.Features.Admission.Models.Dtos;
using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Admission.Features.Admission.Endpoints;

public static class WardRoomEndpoints
{
    public static IEndpointRouteBuilder MapWardRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/wardrooms").WithTags("Ward Rooms v2");

        // LIST
        g.MapGet("/", async (AdmissionDbContext db, CancellationToken ct) =>
        {
            var items = await db.WardRooms.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.WardRoomId)
                .Select(x => new WardRoomDto(
                    x.WardRoomId, x.WardId, x.RoomTypeId, x.RoomNumber, x.Capacity))
                .ToListAsync(ct);

            return Results.Ok(items);
        });

        // GET by id
        g.MapGet("/{id:long}", async (long id, AdmissionDbContext db, CancellationToken ct) =>
        {
            var x = await db.WardRooms.AsNoTracking()
                .FirstOrDefaultAsync(e => e.WardRoomId == id && !e.IsDeleted, ct);

            return x is null
                ? Results.NotFound()
                : Results.Ok(new WardRoomDto(
                    x.WardRoomId, x.WardId, x.RoomTypeId, x.RoomNumber, x.Capacity));
        });

        // CREATE
        g.MapPost("/", async (WardRoomDto dto, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = new myWardRoom
            {
                WardId = dto.WardId,
                RoomTypeId = dto.RoomTypeId,
                RoomNumber = dto.RoomNumber,
                Capacity = dto.Capacity,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "api"
            };
            db.WardRooms.Add(e);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/api/v2/wardrooms/{e.WardRoomId}",
                new WardRoomDto(e.WardRoomId, e.WardId, e.RoomTypeId, e.RoomNumber, e.Capacity));
        });

        // UPDATE
        g.MapPut("/{id:long}", async (long id, WardRoomDto dto, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = await db.WardRooms.FirstOrDefaultAsync(x => x.WardRoomId == id && !x.IsDeleted, ct);
            if (e is null) return Results.NotFound();

            e.WardId = dto.WardId;
            e.RoomTypeId = dto.RoomTypeId;
            e.RoomNumber = dto.RoomNumber;
            e.Capacity = dto.Capacity;
            e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = "api";
            await db.SaveChangesAsync(ct);

            return Results.Ok(new WardRoomDto(
                e.WardRoomId, e.WardId, e.RoomTypeId, e.RoomNumber, e.Capacity));
        });

        // DELETE (soft)
        g.MapDelete("/{id:long}", async (long id, AdmissionDbContext db, CancellationToken ct) =>
        {
            var e = await db.WardRooms.FirstOrDefaultAsync(x => x.WardRoomId == id && !x.IsDeleted, ct);
            if (e is null) return Results.NotFound();

            e.IsDeleted = true; e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = "api";
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        return app;
    }
}

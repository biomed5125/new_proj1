using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class InstrumentMapEndpoints
{
    // Query
    public static IEndpointRouteBuilder MapInstrumentMapQueryEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/catalog").WithTags("Laboratory");

        g.MapGet("/instrument-maps", async (
            long? deviceId,
            LabDbContext db,
            CancellationToken ct) =>
        {
            var items = await
                (from m in db.InstrumentTestMaps.AsNoTracking()
                 join t in db.LabTests.AsNoTracking() on m.LabTestId equals t.LabTestId
                 where deviceId == null || m.DeviceId == deviceId
                 orderby m.DeviceId, t.Code
                 select new InstrumentTestMapListItemDto(
                     m.InstrumentTestMapId,
                     m.DeviceId,
                     m.LabTestId,
                     t.Code ?? "",
                     t.Name ?? "",
                     m.InstrumentTestCode ?? ""))
                .ToListAsync(ct);

            return Results.Ok(items);
        });

        return app;
    }

    // Mutations
    public static IEndpointRouteBuilder MapInstrumentMapMutationEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/catalog/instrument-maps").WithTags("Laboratory");

        g.MapPost("/upsert", async (
            [FromBody] UpsertInstrumentTestMapDto dto,
            LabDbContext db,
            CancellationToken ct) =>
        {
            var code = (dto.InstrumentTestCode ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(code))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["instrumentTestCode"] = new[] { "Instrument test code is required." }
                });

            var labCode = await db.LabTests.AsNoTracking()
                .Where(x => x.LabTestId == dto.LabTestId)
                .Select(x => x.Code)
                .SingleOrDefaultAsync(ct);

            if (string.IsNullOrWhiteSpace(labCode))
                return Results.BadRequest("Unknown LabTestId.");

            var m = await db.InstrumentTestMaps
                .FirstOrDefaultAsync(x => x.DeviceId == dto.DeviceId && x.LabTestId == dto.LabTestId, ct);

            if (m is null)
            {
                m = new myInstrumentTestMap
                {
                    DeviceId = dto.DeviceId,
                    LabTestId = dto.LabTestId,
                    LabTestCode = labCode!,
                    InstrumentTestCode = code,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "api"
                };
                db.InstrumentTestMaps.Add(m);
            }
            else
            {
                m.LabTestCode = labCode!;
                m.InstrumentTestCode = code;
                m.UpdatedAt = DateTime.UtcNow;
                m.UpdatedBy = "api";
            }

            await db.SaveChangesAsync(ct);
            return Results.Ok(new
            {
                m.InstrumentTestMapId,
                m.DeviceId,
                m.LabTestId,
                m.LabTestCode,
                m.InstrumentTestCode
            });
        });

        return app;
    }
}

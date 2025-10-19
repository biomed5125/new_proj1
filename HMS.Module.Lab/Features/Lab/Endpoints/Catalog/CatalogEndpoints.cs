using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapLabCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/catalog").WithTags("Laboratory");

        // Tests
        g.MapGet("/tests", async (LabDbContext db, CancellationToken ct) =>
            await db.LabTests.Include(t => t.SpecimenType)
                .Select(t => new LabTestDto(t.LabTestId, t.Code, t.Name, t.SpecimenType.Name, t.Unit, t.Price, t.TatMinutes, t.IsActive))
                .ToListAsync(ct));

        // POST /tests (create)
        g.MapPost("/tests", async (
            [FromBody] UpsertTestDto dto,
            IValidator<UpsertTestDto> v,
            ILabCatalogService svc,
            LabDbContext db,
            CancellationToken ct) =>
        {
            // 🔹 Auto-generate code if blank
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                var baseCode = CodeGen.FromName(dto.Name);
                var unique = await CodeGen.EnsureUniqueLabTestCodeAsync(db, baseCode, ct);
                dto = dto with { Code = unique };
            }

            var val = await v.ValidateAsync(dto, ct);
            if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());

            var t = await svc.UpsertTestAsync(dto, ct);
            return Results.Ok(new { t.LabTestId, t.Code, t.Name });
        });

        // Panels
        g.MapGet("/panels", async (LabDbContext db, CancellationToken ct) =>
            await db.LabPanels
                .Include(p => p.Items).ThenInclude(i => i.Test)
                .Select(p => new PanelDto(
                    p.LabPanelId,
                    p.Code,
                    p.Name,
                    p.IsActive,
                    p.Items.OrderBy(i => i.SortOrder).Select(i => i.LabTestId).ToList()
                ))
                .ToListAsync(ct));

        // POST /panels (create)
        g.MapPost("/panels", async (
            [FromBody] UpsertPanelDto dto,
            IValidator<UpsertPanelDto> v,
            ILabCatalogService svc,
            LabDbContext db,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                var baseCode = CodeGen.FromName(dto.Name);      // e.g., "BASICCHEM"
                var unique = await CodeGen.EnsureUniquePanelCodeAsync(db, baseCode, ct);
                dto = dto with { Code = unique };
            }

            var val = await v.ValidateAsync(dto, ct);
            if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());

            var p = await svc.UpsertPanelAsync(dto, ct);
            return Results.Ok(new { p.LabPanelId, p.Code, p.Name });
        });

        // Specimens
        g.MapGet("/specimens", async (LabDbContext db, CancellationToken ct) =>
            await db.SpecimenTypes
                .Select(s => new SpecimenTypeDto(s.SpecimenTypeId, s.Name, s.Code))
                .ToListAsync(ct));
        // Features/Lab/Endpoints/CatalogEndpoints.cs  (ADD to MapLabCatalogEndpoints group `g`)
        g.MapGet("/tests/{id:long}", async (long id, LabDbContext db, CancellationToken ct) =>
        {
            var t = await db.LabTests
                .Include(x => x.SpecimenType)
                .Include(x => x.DefaultReferenceRange)
                .FirstOrDefaultAsync(x => x.LabTestId == id, ct);
            if (t is null) return Results.NotFound();

            var mappingsCount = await db.InstrumentTestMaps
                .CountAsync(m => m.LabTestId == id && !m.IsDeleted, ct);

            return Results.Ok(new
            {
                t.LabTestId,
                t.Code,
                t.Name,
                t.SpecimenTypeId,
                specimen = t.SpecimenType.Name,
                t.Unit,
                t.Price,
                t.TatMinutes,
                t.IsActive,
                t.RefLow,
                t.RefHigh,
                defaultRange = t.DefaultReferenceRange is null ? null : new
                {
                    t.DefaultReferenceRange.ReferenceRangeId,
                    t.DefaultReferenceRange.RefLow,
                    t.DefaultReferenceRange.RefHigh,
                    t.DefaultReferenceRange.Note
                },
                isMapped = mappingsCount > 0,
                mappingsCount
            });
        });


        // PUT /tests/{id} (update – keep code immutable unless you want to allow changes)
        g.MapPut("/tests/{id:long}", async (
    long id,
    [FromBody] UpsertTestDto dto,
    IValidator<UpsertTestDto> v,
    LabDbContext db,
    ILabCatalogService svc,
    CancellationToken ct) =>
        {
            var existing = await db.LabTests.AsNoTracking().FirstOrDefaultAsync(x => x.LabTestId == id, ct);
            if (existing is null) return Results.NotFound();

            // If someone tries to change the Code AND test is mapped → reject
            var mappedCount = await db.InstrumentTestMaps.CountAsync(m => m.LabTestId == id && !m.IsDeleted, ct);
            if (mappedCount > 0 && !string.Equals(dto.Code, existing.Code, StringComparison.OrdinalIgnoreCase))
            {
                return Results.Conflict(new
                {
                    message = "Code is locked because this test is mapped to an instrument.",
                    currentCode = existing.Code,
                    mappingsCount = mappedCount
                });
            }

            // Preserve code if blank or same; never let it drift
            var safeDto = string.IsNullOrWhiteSpace(dto.Code)
                ? dto with { Code = existing.Code }
                : dto with { Code = existing.Code };

            var val = await v.ValidateAsync(safeDto, ct);
            if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());

            var t = await svc.UpsertTestAsync(safeDto, ct);
            return Results.Ok(new { t.LabTestId, t.Code, t.Name });
        });


        g.MapDelete("/tests/{id:long}", async (long id, ILabCatalogService svc, CancellationToken ct) =>
            await svc.DeleteTestAsync(id, ct) ? Results.NoContent() : Results.NotFound());

        g.MapPost("/tests/{id:long}/toggle", async (long id, [FromQuery] bool active, ILabCatalogService svc, CancellationToken ct) =>
            await svc.ToggleTestAsync(id, active, ct) ? Results.NoContent() : Results.NotFound());

        // ----- Panels -----
        g.MapGet("/panels/{id:long}", async (long id, LabDbContext db, CancellationToken ct) =>
        {
            var p = await db.LabPanels
                .Include(x => x.Items).ThenInclude(i => i.Test)
                .FirstOrDefaultAsync(x => x.LabPanelId == id, ct);
            if (p is null) return Results.NotFound();

            return Results.Ok(new PanelDto(
                p.LabPanelId, p.Code, p.Name, p.IsActive,
                p.Items.OrderBy(i => i.SortOrder).Select(i => i.LabTestId).ToList()
            ));
        });

        // PUT /panels/{id}
        g.MapPut("/panels/{id:long}", async (
            long id,
            [FromBody] UpsertPanelDto dto,
            IValidator<UpsertPanelDto> v,
            LabDbContext db,
            ILabCatalogService svc,
            CancellationToken ct) =>
        {
            var existing = await db.LabPanels.AsNoTracking().FirstOrDefaultAsync(x => x.LabPanelId == id, ct);
            if (existing is null) return Results.NotFound();

            if (string.IsNullOrWhiteSpace(dto.Code)) dto = dto with { Code = existing.Code };

            var val = await v.ValidateAsync(dto, ct);
            if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());

            var p = await svc.UpsertPanelAsync(dto with { Code = existing.Code }, ct);
            return Results.Ok(new { p.LabPanelId, p.Code, p.Name });
        });

        g.MapDelete("/panels/{id:long}", async (long id, ILabCatalogService svc, CancellationToken ct) =>
            await svc.DeletePanelAsync(id, ct) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}

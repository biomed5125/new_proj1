// Features/Lab/Endpoints/LabPreanalyticalEndpoints.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class LabPreanalyticalEndpoints
{
    public sealed record Dto(
        long LabRequestId,
        bool IsDiabetic,
        bool TookAntibioticLast3Days,
        int? FastingHours,
        bool HasAllergy,
        string? AllergyNotes,
        string? ThyroidStatus,
        bool HasAnemia,
        bool HasFattyLiver,
        bool HasHighCholesterol,
        bool Dialysis,
        bool CardiacAttackHistory,
        bool Pacemaker,
        int? BloodPressureSys,
        int? BloodPressureDia,
        int? PulseBpm,
        string? Notes
    );

    public static IEndpointRouteBuilder MapLabPreanalytical(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/preanalytics").WithTags("Laboratory");

        // GET /api/v1/lab/preanalytics/{labRequestId}
        g.MapGet("/{labRequestId:long}", async (long labRequestId, LabDbContext db, CancellationToken ct) =>
        {
            var x = await db.Set<myLabPreanalytical>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.LabRequestId == labRequestId, ct);

            if (x is null) return Results.Ok(new Dto(
                labRequestId, false, false, null, false, null, "None",
                false, false, false, false, false, false, null, null, null, null));

            return Results.Ok(new Dto(
                x.LabRequestId, x.IsDiabetic, x.TookAntibioticLast3Days, x.FastingHours,
                x.HasAllergy, x.AllergyNotes, x.ThyroidStatus ?? "None", x.HasAnemia, x.HasFattyLiver,
                x.HasHighCholesterol, x.Dialysis, x.CardiacAttackHistory, x.Pacemaker,
                x.BloodPressureSys, x.BloodPressureDia, x.PulseBpm, x.Notes));
        });

        // POST /api/v1/lab/preanalytics  (upsert)
        g.MapPost("", async ([FromBody] Dto dto, LabDbContext db, CancellationToken ct) =>
        {
            var x = await db.Set<myLabPreanalytical>().FirstOrDefaultAsync(p => p.LabRequestId == dto.LabRequestId, ct);
            if (x is null)
            {
                x = new myLabPreanalytical { LabRequestId = dto.LabRequestId };
                db.Add(x);
            }

            x.IsDiabetic = dto.IsDiabetic;
            x.TookAntibioticLast3Days = dto.TookAntibioticLast3Days;
            x.FastingHours = dto.FastingHours;
            x.HasAllergy = dto.HasAllergy;
            x.AllergyNotes = dto.AllergyNotes;
            x.ThyroidStatus = string.IsNullOrWhiteSpace(dto.ThyroidStatus) ? "None" : dto.ThyroidStatus;
            x.HasAnemia = dto.HasAnemia;
            x.HasFattyLiver = dto.HasFattyLiver;
            x.HasHighCholesterol = dto.HasHighCholesterol;
            x.Dialysis = dto.Dialysis;
            x.CardiacAttackHistory = dto.CardiacAttackHistory;
            x.Pacemaker = dto.Pacemaker;
            x.BloodPressureSys = dto.BloodPressureSys;
            x.BloodPressureDia = dto.BloodPressureDia;
            x.PulseBpm = dto.PulseBpm;
            x.Notes = dto.Notes;

            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        return g;
    }
}

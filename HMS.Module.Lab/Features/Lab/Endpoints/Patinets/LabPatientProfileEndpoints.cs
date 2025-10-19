
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints.Patinets
{
    public static class LabPatientProfileEndpoints
    {
        public static IEndpointRouteBuilder MapLabPatientProfileEndpoints(this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/api/v1/lab/patientprofile").WithTags("Laboratory");

            // GET – returns profile or a default “empty” one (so UI can render chips)
            g.MapGet("/{patientId:long}", async (long patientId, LabDbContext db, CancellationToken ct) =>
            {
                var row = await db.LabPatientProfiles.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.LabPatientId == patientId, ct);

                if (row is null)
                {
                    // ensure patient exists (optional)
                    var exists = await db.LabPatients.AnyAsync(p => p.LabPatientId == patientId, ct);
                    if (!exists) return Results.NotFound("Patient not found.");

                    row = new myLabPatientProfile { LabPatientId = patientId };
                }
                return Results.Ok(new
                {
                    row.LabPatientId,
                    row.Diabetic,
                    row.Thyroid,
                    row.ChronicAnemia,
                    row.Dialysis,
                    row.Pacemaker,
                    row.CardiacHistory,
                    row.Allergy,
                    row.AllergyNotes,
                    row.FattyLiver,
                    row.HighCholesterol,
                    row.UpdatedAt,
                    row.UpdatedBy
                });
            });

            // PUT – upsert profile
            g.MapPut("/{patientId:long}", async (
                long patientId,
                [FromBody] myLabPatientProfile dto,
                LabDbContext db,
                CancellationToken ct) =>
            {
                if (patientId != dto.LabPatientId) return Results.BadRequest("ID mismatch.");

                var p = await db.LabPatientProfiles.FirstOrDefaultAsync(x => x.LabPatientId == patientId, ct);
                if (p is null)
                {
                    // verify patient exists
                    var exists = await db.LabPatients.AnyAsync(x => x.LabPatientId == patientId, ct);
                    if (!exists) return Results.NotFound("Patient not found.");

                    p = new myLabPatientProfile { LabPatientId = patientId };
                    db.LabPatientProfiles.Add(p);
                }

                p.Diabetic = dto.Diabetic;
                p.Thyroid = dto.Thyroid;
                p.ChronicAnemia = dto.ChronicAnemia;
                p.Dialysis = dto.Dialysis;
                p.Pacemaker = dto.Pacemaker;
                p.CardiacHistory = dto.CardiacHistory;
                p.Allergy = dto.Allergy;
                p.AllergyNotes = string.IsNullOrWhiteSpace(dto.AllergyNotes) ? null : dto.AllergyNotes.Trim();
                p.FattyLiver = dto.FattyLiver;
                p.HighCholesterol = dto.HighCholesterol;
                p.UpdatedAt = DateTime.UtcNow;
                p.UpdatedBy = "ui";

                await db.SaveChangesAsync(ct);
                return Results.NoContent();
            });

            return app;
        }
    }

}

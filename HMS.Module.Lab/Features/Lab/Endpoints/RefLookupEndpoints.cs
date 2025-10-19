// HMS.Module.Lab/Features/Lab/Endpoints/RefLookupsEndpoints.cs
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class RefLookupsEndpoints
{
    public sealed record PatientPick(long Id, string Name, string? Mrn, string? Sex, DateTime? Dob);
    public sealed record DoctorPick(long Id, string Name, string? Specialty);

    public static IEndpointRouteBuilder MapRefLookups(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/refs").WithTags("Laboratory");

        g.MapGet("/patients", async (string q, int? take, LabDbContext db, CancellationToken ct) =>
        {
            q = (q ?? "").Trim();
            var lim = Math.Clamp(take ?? 10, 1, 50);
            if (q.Length < 2) return Results.Ok(Array.Empty<PatientPick>());

            var rows = await db.LabPatients.AsNoTracking()
                .Where(p => p.FullName.Contains(q) || (p.Mrn != null && p.Mrn.Contains(q)))
                .OrderBy(p => p.FullName)
                .Take(lim)
                .Select(p => new PatientPick(p.LabPatientId, p.FullName, p.Mrn, p.Sex, p.DateOfBirth))
                .ToListAsync(ct);

            return Results.Ok(rows);
        });

        g.MapGet("/doctors", async (string q, int? take, LabDbContext db, CancellationToken ct) =>
        {
            q = (q ?? "").Trim();
            var lim = Math.Clamp(take ?? 10, 1, 50);
            if (q.Length < 2) return Results.Ok(Array.Empty<DoctorPick>());

            var rows = await db.LabDoctors.AsNoTracking()
                .Where(d => d.FullName.Contains(q))
                .OrderBy(d => d.FullName)
                .Take(lim)
                .Select(d => new DoctorPick(d.LabDoctorId, d.FullName, d.Specialty))
                .ToListAsync(ct);

            return Results.Ok(rows);
        });

        return app;
    }
}

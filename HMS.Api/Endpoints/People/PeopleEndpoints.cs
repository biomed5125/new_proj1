// Features/People/PeopleEndpoints.cs  (or any API area you prefer)
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Endpoints.People
{
    public static class PeopleEndpoints
    {
        public sealed record PickItem(long Id, string Name);

        public static IEndpointRouteBuilder MapPeopleLookups(this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/api/people");

            // Patients
            g.MapGet("/patients/search", async (string q, LabDbContext db, CancellationToken ct) =>
            {
                q = (q ?? "").Trim();
                if (q.Length < 2) return Results.Ok(Array.Empty<PickItem>());

                var rows = await db.LabPatients.AsNoTracking()
                    .Where(p => p.FullName.Contains(q))
                    .OrderBy(p => p.FullName)
                    .Select(p => new PickItem(p.LabPatientId, p.FullName))
                    .Take(10)
                    .ToListAsync(ct);

                return Results.Ok(rows);
            });

            // Doctors
            g.MapGet("/doctors/search", async (string q, LabDbContext db, CancellationToken ct) =>
            {
                q = (q ?? "").Trim();
                if (q.Length < 2) return Results.Ok(Array.Empty<PickItem>());

                var rows = await db.LabDoctors.AsNoTracking()
                    .Where(d => d.FullName.Contains(q))
                    .OrderBy(d => d.FullName)
                    .Select(d => new PickItem(d.LabDoctorId, d.FullName))
                    .Take(10)
                    .ToListAsync(ct);

                return Results.Ok(rows);
            });

            return app;
        }
    }

}

using HMS.Module.Patient.Infrastructure.Persistence;
using HMS.Module.Patient.Features.Patient.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Module.Patient.Infrastructure.Persistence.Seed;

public static class PatientDbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("PatientDbSeeder");
        var db = sp.GetRequiredService<PatientDbContext>();

        await db.Database.MigrateAsync(ct);

        if (await db.Patients.AnyAsync(ct))
        {
            logger.LogInformation("Patient DB already has data. Skipping seed.");
            return;
        }

        var now = DateTime.UtcNow;

        var patients = new[]
        {
            new myPatient
            {
                Mrn = "P00000001",
                FirstName = "Ali",
                LastName  = "Hassan",
                DateOfBirth = new DateTime(1985, 5, 14),
                Gender = "M", Phone = "555-0100", Email = "ali@example.com",
                CreatedAt = now, CreatedBy = "seed"
            },
            new myPatient
            {
                Mrn = "P00000002",
                FirstName = "Mona",
                LastName  = "Kamel",
                DateOfBirth = new DateTime(1992, 11, 2),
                Gender = "F", Phone = "555-0101", Email = "mona@example.com",
                CreatedAt = now, CreatedBy = "seed"
            }
        };

        db.Patients.AddRange(patients);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} patients.", patients.Length);
    }
}

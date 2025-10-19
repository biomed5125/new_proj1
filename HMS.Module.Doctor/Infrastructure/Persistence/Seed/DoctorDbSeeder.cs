// Infrastructure/Persistence/Seed/DoctorDbSeeder.cs
using HMS.Module.Doctor.Features.Doctor.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Module.Doctor.Infrastructure.Persistence.Seed;

public static class DoctorDbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DoctorDbSeeder");
        await using var db = sp.GetRequiredService<DoctorDbContext>();

        await db.Database.MigrateAsync(ct);

        if (await db.Doctors.AnyAsync(ct))
        {
            log.LogInformation("Doctor DB already has data. Skipping seed.");
            return;
        }

        var now = DateTime.UtcNow;
        db.Doctors.AddRange(
            new myDoctor { FirstName = "Omar", LastName = "Salem", LicenseNumber = "LIC-1001", Specialty = "Internal Medicine", Phone = "0781111111", Email = "omar.salem@hms.dev", IsActive = true, HireDateUtc = now.AddYears(-3), CreatedAt = now, CreatedBy = "seed" },
            new myDoctor { FirstName = "Mariam", LastName = "Kareem", LicenseNumber = "LIC-1002", Specialty = "Radiology", Phone = "0782222222", Email = "mariam.kareem@hms.dev", IsActive = true, HireDateUtc = now.AddYears(-2), CreatedAt = now, CreatedBy = "seed" }
        );

        await db.SaveChangesAsync(ct);
        log.LogInformation("Seeded Doctors.");
    }
}

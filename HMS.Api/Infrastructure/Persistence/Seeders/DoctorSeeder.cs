using HMS.Api.Features.Doctor.Models.Entities;
using System.Numerics;

namespace HMS.Api.Infrastructure.Persistence.Seed;

public static class DoctorSeeder
{
    public static void Seed(HmsDbContext db)
    {
        if (db.Set<myDoctor>().Any(d => !d.IsDeleted)) return;

        var now = DateTime.UtcNow;
        db.Set<myDoctor>().AddRange(
            new myDoctor { FirstName = "Omar", LastName = "Salem", LicenseNumber = "LIC-1001", Specialty = "Internal Medicine", Phone = "0781111111", Email = "omar.salem@hms.dev", IsActive = true, HireDate = now.AddYears(-3), CreatedAt = now, CreatedBy = "seed" },
            new myDoctor { FirstName = "Mariam", LastName = "Kareem", LicenseNumber = "LIC-1002", Specialty = "Radiology", Phone = "0782222222", Email = "mariam.kareem@hms.dev", IsActive = true, HireDate = now.AddYears(-2), CreatedAt = now, CreatedBy = "seed" }
        );
        db.SaveChanges();
    }
}

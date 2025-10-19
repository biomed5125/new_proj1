using HMS.Api.Features.Patient.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Infrastructure.Persistence.Seeders;

public static class PatientSeeder
{
    public static async Task SeedPatientAsync(DbContext db)
    {
        if (!await db.Set<myPatient>().AnyAsync())
        {
            db.Set<myPatient>().AddRange(
                new myPatient { FirstName = "Ali", LastName = "Hassan", Phone = "0770000001", CreatedAt = DateTime.UtcNow },
                new myPatient { FirstName = "Sara", LastName = "Omar", Phone = "0770000002", CreatedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();
        }
    }
}

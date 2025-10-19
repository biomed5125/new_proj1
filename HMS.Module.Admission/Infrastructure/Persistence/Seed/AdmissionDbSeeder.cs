using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Features.Admission.Models.Enums;
using HMS.Module.Admission.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Optional: pull a real patient id, if the Patient module is wired up
using HMS.Module.Patient.Infrastructure.Persistence;

namespace HMS.Module.Admission.Infrastructure.Persistence.Seed;

public static class AdmissionDbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        await using var db = sp.GetRequiredService<AdmissionDbContext>();
        // Migrations are run elsewhere (Program.cs) to avoid sequence-duplication issues.

        var now = DateTime.UtcNow;

        // ---- Wards ----
        if (!await db.Wards.AnyAsync(ct))
        {
            db.Wards.AddRange(
                new myWard { Name = "General", CreatedAt = now, CreatedBy = "seed" },
                new myWard { Name = "Surgical", CreatedAt = now, CreatedBy = "seed" }
            );
            await db.SaveChangesAsync(ct);
        }

        // ---- RoomTypes ----
        if (!await db.RoomTypes.AnyAsync(ct))
        {
            db.RoomTypes.AddRange(
                new myRoomType { Name = "Standard", DailyRate = 50m, CreatedAt = now, CreatedBy = "seed" },
                new myRoomType { Name = "Private", DailyRate = 120m, CreatedAt = now, CreatedBy = "seed" }
            );
            await db.SaveChangesAsync(ct);
        }

        // ---- WardRooms ----
        if (!await db.WardRooms.AnyAsync(ct))
        {
            var wardId = await db.Wards
                .Where(w => w.Name == "General")
                .Select(w => w.WardId)
                .FirstAsync(ct);

            var rtId = await db.RoomTypes
                .Where(r => r.Name == "Standard")
                .Select(r => r.RoomTypeId)
                .FirstAsync(ct);

            db.WardRooms.Add(new myWardRoom
            {
                WardId = wardId,
                RoomTypeId = rtId,
                RoomNumber = "101",
                Capacity = 2,
                CreatedAt = now,
                CreatedBy = "seed"
            });
            await db.SaveChangesAsync(ct);
        }

        // ---- Seed ONE Admission if none exist ----
        if (!await db.Admissions.AnyAsync(ct))
        {
            // Try to use a real PatientId from the Patient DB (if that module is present)
            long patientId = 0;
            var patientDb = sp.GetService<PatientDbContext>();
            if (patientDb is not null)
            {
                patientId = await patientDb.Patients
                    .AsNoTracking()
                    .OrderBy(p => p.PatientId)
                    .Select(p => p.PatientId)
                    .FirstOrDefaultAsync(ct);
            }
            if (patientId == 0)
                patientId = 2025100000001L; // fallback: adjust to an existing id if you prefer

            // Use the first ward room (we just seeded one)
            var wardRoomId = await db.WardRooms
                .AsNoTracking()
                .OrderBy(r => r.WardRoomId)
                .Select(r => r.WardRoomId)
                .FirstAsync(ct);

            db.Admissions.Add(new myAdmission
            {
                PatientId = patientId,
                WardRoomId = wardRoomId,
                AdmittedAtUtc = now,
                Status = AdmissionStatus.Admitted,
                DiagnosisOnAdmission = "Seed admission",
                EncounterNo = $"E{now:yyyyMMddHHmmssfff}", // simple generated encounter number
                Notes = "Seed data",
                CreatedAt = now,
                CreatedBy = "seed"
            });

            await db.SaveChangesAsync(ct);
        }
    }
}

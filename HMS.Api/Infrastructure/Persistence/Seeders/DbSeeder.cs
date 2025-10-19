using HMS.Api.Features.Admission.Models.Entities;
using HMS.Api.Features.Admission.Models.Enums;
using HMS.Api.Features.Billing.Models.Entities;
using HMS.Api.Features.Patient.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static void Seed(HmsDbContext db)
    {
        var now = DateTime.UtcNow;

        // ---- Patients (for tests) ----
        if (!db.Set<myPatient>().Any(p => !p.IsDeleted))
        {
            db.Set<myPatient>().AddRange(
                new myPatient { FirstName = "Ali", LastName = "Hassan", Phone = "0770000001", CreatedAt = now, CreatedBy = "seed" },
                new myPatient { FirstName = "Sara", LastName = "Yousef", Phone = "0770000002", CreatedAt = now, CreatedBy = "seed" }
            );
            db.SaveChanges();
        }

        // ---- Wards ----
        if (!db.Set<myWard>().Any())
        {
            db.Set<myWard>().AddRange(
                new myWard { Name = "General", CreatedAt = now, CreatedBy = "seed" },
                new myWard { Name = "Surgical", CreatedAt = now, CreatedBy = "seed" }
            );
            db.SaveChanges();
        }

        var wardGeneralId = db.Set<myWard>().Where(w => w.Name == "General").Select(w => w.WardId).First();
        var wardSurgicalId = db.Set<myWard>().Where(w => w.Name == "Surgical").Select(w => w.WardId).First();

        //----RoomTypes----
        if (!db.Set<myRoomType>().Any())
        {
            db.Set<myRoomType>().AddRange(
                new myRoomType { Name = "Standard", DailyRate = 50.00m, CreatedAt = now, CreatedBy = "seed" },
                new myRoomType { Name = "Private", DailyRate = 120.00m, CreatedAt = now, CreatedBy = "seed" }
            );
            db.SaveChanges();
        }

        var rtStandardId = db.Set<myRoomType>().Where(r => r.Name == "Standard").Select(r => r.RoomTypeId).First();
        var rtPrivateId = db.Set<myRoomType>().Where(r => r.Name == "Private").Select(r => r.RoomTypeId).First();

        //----WardRooms(unique per Ward + RoomNumber)----
        if (!db.Set<myWardRoom>().Any())
        {
            db.Set<myWardRoom>().AddRange(
                new myWardRoom { WardId = wardGeneralId, RoomTypeId = rtStandardId, RoomNumber = "101", Capacity = 2, CreatedAt = now, CreatedBy = "seed" },
                new myWardRoom { WardId = wardGeneralId, RoomTypeId = rtStandardId, RoomNumber = "102", Capacity = 4, CreatedAt = now, CreatedBy = "seed" },
                new myWardRoom { WardId = wardSurgicalId, RoomTypeId = rtPrivateId, RoomNumber = "201", Capacity = 1, CreatedAt = now, CreatedBy = "seed" }
            );
            db.SaveChanges();
        }

        // ---- Optional: one sample admission for Ali in room 101 ----
        var aliId = db.Set<myPatient>().Where(p => p.FirstName == "Ali" && p.LastName == "Hassan" && !p.IsDeleted)
                                     .Select(p => p.PatientId).First();
        var room101Id = db.Set<myWardRoom>().Where(r => r.WardId == wardGeneralId && r.RoomNumber == "101")
                                          .Select(r => r.WardRoomId).First();

        var hasOpenAdmission = db.Set<myAdmission>().Any(a => !a.IsDeleted && a.PatientId == aliId && a.DischargedAtUtc == null);
        if (!hasOpenAdmission)
        {
            db.Set<myAdmission>().Add(new myAdmission
            {
                PatientId = aliId,
                WardRoomId = room101Id,
                AdmittedAtUtc = now,
                Status = AdmissionStatus.Admitted,
                DiagnosisOnAdmission = "Pneumonia",
                Notes = "Seed admission",
                CreatedAt = now,
                CreatedBy = "seed"
            });
            db.SaveChanges();
        }
    }
}

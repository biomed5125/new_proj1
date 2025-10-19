//using HMS.Api.Features.Admission.Models.Entities;
//using HMS.Api.Features.Patient.Models.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Api.Infrastructure.Persistence.Seeders;

//public static class AdmissionSeeder
//{
//    public static async Task SeedAdmissionAsync(DbContext db)
//    {
//        // ---- Optional: one sample admission for Ali in room 101 ----
//        var aliId = db.Set<myPatient>().Where(p => p.FirstName == "Ali" && p.LastName == "Hassan" && !p.IsDeleted)
//                                     .Select(p => p.PatientId).First();
//        var room101Id = db.Set<myWardRoom>().Where(r => r.WardId == wardGeneralId && r.RoomNumber == "101")
//                                          .Select(r => r.WardRoomId).First();

//        var hasOpenAdmission = db.Set<myAdmission>().Any(a => !a.IsDeleted && a.PatientId == aliId && a.DischargedAtUtc == null);
//        if (!hasOpenAdmission)
//        {
//            db.Set<myAdmission>().Add(new myAdmission
//            {
//                PatientId = aliId,
//                WardRoomId = room101Id,
//                AdmittedAtUtc = DateTime.UtcNow,
//                Status = AdmissionStatus.Admitted,
//                DiagnosisOnAdmission = "Pneumonia",
//                Notes = "Seed admission",
//                CreatedAt = DateTime.UtcNow,
//                CreatedBy = "seed"
//            });
//            db.SaveChanges();
//        }
//    }
//}

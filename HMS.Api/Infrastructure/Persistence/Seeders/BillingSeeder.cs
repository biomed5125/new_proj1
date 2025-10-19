using HMS.Api.Features.Billing.Models.Entities;
using HMS.Api.Features.Billing.Models.Enums;

namespace HMS.Api.Infrastructure.Persistence.Seed;

public static class BillingSeeder
{
    public static void Seed(HmsDbContext db)
    {
        var now = DateTime.UtcNow;

        // --- Services catalog (idempotent) ---
        if (!db.Set<myHospitalService>().Any(s => !s.IsDeleted))
        {
            db.Set<myHospitalService>().AddRange(
                // LAB
                new myHospitalService { Code = "LAB-CBC", Name = "Complete Blood Count", Category = ServiceCategory.Lab, Unit = "test", DefaultPrice = 8.00m, CreatedAt = now, CreatedBy = "seed" },
                new myHospitalService { Code = "LAB-BMP", Name = "Basic Metabolic Panel", Category = ServiceCategory.Lab, Unit = "test", DefaultPrice = 12.00m, CreatedAt = now, CreatedBy = "seed" },
                new myHospitalService { Code = "LAB-LFT", Name = "Liver Function Tests", Category = ServiceCategory.Lab, Unit = "test", DefaultPrice = 10.00m, CreatedAt = now, CreatedBy = "seed" },

                // RADIOLOGY
                new myHospitalService { Code = "RAD-CXR", Name = "Chest X-Ray", Category = ServiceCategory.Radiology, Unit = "film", DefaultPrice = 20.00m, CreatedAt = now, CreatedBy = "seed" },

                // PROCEDURE
                new myHospitalService { Code = "PROC-DRESS", Name = "Wound Dressing", Category = ServiceCategory.Procedure, Unit = "procedure", DefaultPrice = 15.00m, CreatedAt = now, CreatedBy = "seed" },

                // PHARMACY (example)
                new myHospitalService { Code = "PHARM-AMOX500", Name = "Amoxicillin 500mg", Category = ServiceCategory.Pharmacy, Unit = "pack", DefaultPrice = 5.00m, CreatedAt = now, CreatedBy = "seed" },

                // OTHER / ADMIN
                new myHospitalService { Code = "ADMIN-REG", Name = "Registration Fee", Category = ServiceCategory.Other, Unit = "each", DefaultPrice = 2.50m, CreatedAt = now, CreatedBy = "seed" }
            );
            db.SaveChanges();
        }

        // --- Optional sample Charge (idempotent) ---
        // This just demonstrates the ledger; workflow charges should be created by services.
        var svcReg = db.Set<myHospitalService>().FirstOrDefault(s => s.Code == "ADMIN-REG" && s.IsActive && !s.IsDeleted);
        var patientAliId = db.Set<HMS.Api.Features.Patient.Models.Entities.myPatient>()
                             .Where(p => !p.IsDeleted && p.FirstName == "Ali" && p.LastName == "Hassan")
                             .Select(p => p.PatientId)
                             .FirstOrDefault();

        long? openAdmissionId = db.Set<HMS.Api.Features.Admission.Models.Entities.myAdmission>()
                                  .Where(a => !a.IsDeleted && a.PatientId == patientAliId && a.DischargedAtUtc == null)
                                  .Select(a => (long?)a.AdmissionId)
                                  .FirstOrDefault();

        if (svcReg != null && patientAliId != 0)
        {
            // unique key on (Source, SourceId) → use Source=Other, SourceId=1 for seed
            bool exists = db.Set<myCharge>().Any(c => !c.IsDeleted && c.Source == ChargeSource.Other && c.SourceId == 1);
            if (!exists)
            {
                var amount = svcReg.DefaultPrice;
                db.Set<myCharge>().Add(new myCharge
                {
                    PatientId = patientAliId,
                    AdmissionId = openAdmissionId, // null if outpatient
                    Source = ChargeSource.Other,
                    SourceId = 1,
                    ServiceId = svcReg.ServiceId,
                    Quantity = 1,
                    UnitPrice = amount,
                    Total = amount,
                    Status = ChargeStatus.Pending,
                    CreatedAt = now,
                    CreatedBy = "seed"
                });
                db.SaveChanges();
            }
        }

        // --- Example (COMMENTED): how it will look for a Lab request charge
        // if you have a LabRequest entity later:
        //
        // var lab = db.Set<LabRequest>().FirstOrDefault();
        // var svcCbc = db.Set<HospitalService>().FirstOrDefault(s => s.Code=="LAB-CBC");
        // if (lab != null && svcCbc != null &&
        //     !db.Set<Charge>().Any(c => c.Source==ChargeSource.Lab && c.SourceId==lab.LabRequestId))
        // {
        //     db.Set<Charge>().Add(new Charge {
        //         PatientId = lab.PatientId,
        //         AdmissionId = lab.AdmissionId,
        //         Source = ChargeSource.Lab,
        //         SourceId = lab.LabRequestId,
        //         ServiceId = svcCbc.ServiceId,
        //         Quantity = 1, UnitPrice = svcCbc.DefaultPrice, Total = svcCbc.DefaultPrice,
        //         CreatedAt = now, CreatedBy = "seed"
        //     });
        //     db.SaveChanges();
        // }
    }
}

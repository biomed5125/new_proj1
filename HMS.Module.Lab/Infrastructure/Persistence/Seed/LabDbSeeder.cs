//// HMS.Module.Lab/Infrastructure/Persistence/Seed/LabDbSeeder.cs
//using HMS.Module.Lab.Features.Lab.Models.Entities;
//using HMS.Module.Lab.Features.Lab.Models.Enums;
//using HMS.Module.Lab.Infrastructure.Persistence;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace HMS.Module.Lab.Infrastructure.Persistence.Seed;

//public static class LabDbSeeder
//{
//    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
//    {
//        var cfg = sp.GetRequiredService<IConfiguration>();
//        var now = DateTime.UtcNow;

//        // Always resolve DbContext from a scope created by the caller.
//        var db = sp.GetRequiredService<LabDbContext>();

//        // 1) Specimen types (idempotent)
//        if (!await db.SpecimenTypes.AnyAsync(ct))
//        {
//            db.SpecimenTypes.AddRange(
//                new mySpecimenType { Name = "Serum", Code = "SER", CreatedAt = now, CreatedBy = "seed:demo" },
//                new mySpecimenType { Name = "Plasma", Code = "PLAS", CreatedAt = now, CreatedBy = "seed:demo" },
//                new mySpecimenType { Name = "Whole Blood", Code = "WB", CreatedAt = now, CreatedBy = "seed:demo" },
//                new mySpecimenType { Name = "Cerebrospinal fluid", Code = "CSF", CreatedAt = now, CreatedBy = "seed:demo" },
//                new mySpecimenType { Name = "Stool", Code = "FC", CreatedAt = now, CreatedBy = "seed:demo" },
//                new mySpecimenType { Name = "Semen", Code = "SEM", CreatedAt = now, CreatedBy = "seed:demo" }
//            );
//            await db.SaveChangesAsync(ct);
//        }

//        // helper: add a test once
//        async Task EnsureTest(string code, string name, string unit, decimal lo, decimal hi, decimal price)
//        {
//            if (await db.LabTests.AnyAsync(t => t.Code == code && !t.IsDeleted, ct)) return;

//            var serumId = await db.SpecimenTypes
//                .Where(s => s.Name == "Serum" && !s.IsDeleted)
//                .Select(s => s.SpecimenTypeId)
//                .FirstAsync(ct);

//            var rr = new myReferenceRange { RefLow = lo, RefHigh = hi, CreatedAt = now, CreatedBy = "seed:demo" };
//            db.ReferenceRanges.Add(rr);

//            db.LabTests.Add(new myLabTest
//            {
//                Code = code,
//                Name = name,
//                Unit = unit,
//                SpecimenTypeId = serumId,
//                DefaultReferenceRange = rr,
//                Price = price,
//                TatMinutes = 60,
//                IsActive = true,
//                CreatedAt = now,
//                CreatedBy = "seed:demo"
//            });
//            await db.SaveChangesAsync(ct);
//        }

//        // 2) Minimal catalog
//        await EnsureTest("GLU", "Glucose", "mg/dL", 70, 110, 2.75m);
//        await EnsureTest("UREA", "Urea", "mg/dL", 15, 45, 3.20m);
//        await EnsureTest("NA", "Sodium", "mmol/L", 135, 145, 3.00m);
//        await EnsureTest("K", "Potassium", "mmol/L", 3.5m, 5.1m, 3.50m);
//        // --- Chemistry (c311/c501)
//        await EnsureTest("GLU", "Glucose", "mg/dL",10,10,1.5m);
//        await EnsureTest("UREA", "Urea (BUN)", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("CREA", "Creatinine", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("UA", "Uric Acid", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("CHOL", "Cholesterol", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("TG", "Triglycerides", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("HDL", "HDL Cholesterol", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("LDL", "LDL Cholesterol", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("ALT", "ALT (GPT)", "U/L", 10, 10, 1.5m);
//        await EnsureTest("AST", "AST (GOT)", "U/L", 10, 10, 1.5m);
//        await EnsureTest("ALP", "Alkaline Phosphatase", "U/L", 10, 10, 1.5m);
//        await EnsureTest("TBIL", "Bilirubin Total", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("DBIL", "Bilirubin Direct", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("TP", "Total Protein", "g/dL", 10, 10, 1.5m);
//        await EnsureTest("ALB", "Albumin", "g/dL", 10, 10, 1.5m);
//        await EnsureTest("CA", "Calcium", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("MG", "Magnesium", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("PHOS", "Phosphate", "mg/dL", 10, 10, 1.5m);
//        await EnsureTest("CK", "Creatine Kinase", "U/L", 10, 10, 1.5m);
//        await EnsureTest("LDH", "LDH", "U/L", 10, 10, 1.5m);

//        // --- Immuno (e411/e601)
//        await EnsureTest("TSH", "TSH", "µIU/mL", 10, 10, 1.5m);
//        await EnsureTest("FT4", "Free T4", "ng/dL", 10, 10, 1.5m);
//        await EnsureTest("FT3", "Free T3", "pg/mL", 10, 10, 1.5m);
//        await EnsureTest("PRL", "Prolactin", "ng/mL", 10, 10, 1.5m);
//        await EnsureTest("LH", "Luteinizing Hormone", "mIU/mL", 10, 10, 1.5m);
//        await EnsureTest("FSH", "Follicle Stimulating Hormone", "mIU/mL", 10, 10, 1.5m);
//        await EnsureTest("E2", "Estradiol", "pg/mL", 10, 10, 1.5m);
//        await EnsureTest("PROG", "Progesterone", "ng/mL", 10, 10, 1.5m);
//        await EnsureTest("TESTO", "Testosterone", "ng/dL", 10, 10, 1.5m);
//        await EnsureTest("HCG", "β-hCG", "mIU/mL", 10, 10, 1.5m);
//        await EnsureTest("PCT", "Procalcitonin", "ng/mL", 10, 10, 1.5m);
//        await EnsureTest("PSA", "PSA Total", "ng/mL", 10, 10, 1.5m);
//        await EnsureTest("AFP", "Alpha-fetoprotein", "ng/mL", 10, 10, 1.5m);
//        await EnsureTest("CA125", "CA-125", "U/mL", 10, 10, 1.5m);
//        await EnsureTest("CA153", "CA-15-3", "U/mL", 10, 10, 1.5m);
//        await EnsureTest("CA199", "CA-19-9", "U/mL", 10, 10, 1.5m);
//        await EnsureTest("INS", "Insulin", "µIU/mL", 10, 10, 1.5m);
//        await EnsureTest("CP", "C-Peptide", "ng/mL", 10, 10, 1.5m);

//        // 3) Small panel: EL = NA + K
//        if (!await db.LabPanels.AnyAsync(p => p.Code == "EL" && !p.IsDeleted, ct))
//        {
//            var naId = await db.LabTests.Where(t => t.Code == "NA").Select(t => t.LabTestId).FirstAsync(ct);
//            var kId = await db.LabTests.Where(t => t.Code == "K").Select(t => t.LabTestId).FirstAsync(ct);

//            var panel = new myLabPanel
//            {
//                Code = "EL",
//                Name = "Electrolytes",
//                IsActive = true,
//                CreatedAt = now,
//                CreatedBy = "seed:demo"
//            };
//            panel.Items.Add(new myLabPanelItem { LabTestId = naId, SortOrder = 0 });
//            panel.Items.Add(new myLabPanelItem { LabTestId = kId, SortOrder = 1 });
//            db.LabPanels.Add(panel);
//            await db.SaveChangesAsync(ct);
//        }

//        // 4) Demo order + sample (accession used by bench tests)
//        async Task EnsureDemoOrder(string orderNo, string accession, params string[] codes)
//        {
//            long reqId = await db.LabRequests
//                .Where(r => r.OrderNo == orderNo && !r.IsDeleted)
//                .Select(r => r.LabRequestId)
//                .FirstOrDefaultAsync(ct);

//            if (reqId == 0)
//            {
//                var req = new myLabRequest
//                {
//                    PatientId = 1,
//                    OrderNo = orderNo,
//                    Priority = "Routine",
//                    Status = LabRequestStatus.Requested,
//                    CreatedAt = now,
//                    CreatedBy = "seed:demo"
//                };
//                db.LabRequests.Add(req);
//                await db.SaveChangesAsync(ct);
//                reqId = req.LabRequestId;
//            }

//            foreach (var c in codes.Distinct(StringComparer.OrdinalIgnoreCase))
//            {
//                var testId = await db.LabTests.Where(t => t.Code == c).Select(t => t.LabTestId).FirstAsync(ct);
//                var exists = await db.LabRequestItems.AnyAsync(i => i.LabRequestId == reqId && i.LabTestId == testId && !i.IsDeleted, ct);
//                if (!exists)
//                {
//                    db.LabRequestItems.Add(new myLabRequestItem
//                    {
//                        LabRequestId = reqId,
//                        LabTestId = testId,
//                        CreatedAt = now,
//                        CreatedBy = "seed:demo"
//                    });
//                }
//            }
//            await db.SaveChangesAsync(ct);

//            var hasSample = await db.LabSamples.AnyAsync(s => s.LabRequestId == reqId && s.AccessionNumber == accession && !s.IsDeleted, ct);
//            if (!hasSample)
//            {
//                db.LabSamples.Add(new myLabSample
//                {
//                    LabRequestId = reqId,
//                    AccessionNumber = accession,
//                    Status = LabSampleStatus.Collected,
//                    CreatedAt = now,
//                    CreatedBy = "seed:demo"
//                });
//                await db.SaveChangesAsync(ct);
//            }
//        }

//        await EnsureDemoOrder("ORD-SEED-001", "ACC0001", "GLU", "UREA");

//        // 5) Optional: seed instrument→lab maps for your bench device
//        if (long.TryParse(cfg["Communication:Seed:DeviceId"], out var deviceId) && deviceId > 0)
//        {
//            async Task Map(string instr, string lab)
//            {
//                var testId = await db.LabTests.Where(t => t.Code == lab).Select(t => t.LabTestId).FirstAsync(ct);
//                var exists = await db.InstrumentTestMaps.AnyAsync(m => m.DeviceId == deviceId &&
//                                                                       m.InstrumentTestCode == instr &&
//                                                                       !m.IsDeleted, ct);
//                if (!exists)
//                {
//                    var labCode = await db.LabTests.Where(t => t.LabTestId == testId).Select(t => t.Code).FirstAsync(ct);
//                    db.InstrumentTestMaps.Add(new myInstrumentTestMap
//                    {
//                        DeviceId = deviceId,
//                        InstrumentTestCode = instr,
//                        LabTestId = testId,
//                        LabTestCode = labCode,
//                        CreatedAt = now,
//                        CreatedBy = "seed:demo"
//                    });
//                }
//                await db.SaveChangesAsync(ct);

//            }
//            // --- Common chemistry (global)
//            await Map("GLU", "GLU");
//            await Map("UREA", "UREA");
//            await Map("BUN", "UREA");
//            await Map("CREA", "CREA");
//            await Map("UA", "UA");
//            await Map("CHOL", "CHOL");
//            await Map("TG", "TG");
//            await Map("HDL", "HDL");
//            await Map("LDL", "LDL");
//            await Map("ALT", "ALT");
//            await Map("AST", "AST");
//            await Map("ALP", "ALP");
//            await Map("TBIL", "TBIL");
//            await Map("DBIL", "DBIL");
//            await Map("TP", "TP");
//            await Map("ALB", "ALB");
//            await Map("CA", "CA");
//            await Map("MG", "MG");
//            await Map("PHOS", "PHOS");

//            // --- Common immuno (global)
//            await Map("TSH", "TSH");
//            await Map("FT4", "FT4");
//            await Map("FT3", "FT3");
//            await Map("PRL", "PRL");
//            await Map("LH", "LH");
//            await Map("FSH", "FSH");
//            await Map("E2", "E2");
//            await Map("PROG", "PROG");
//            await Map("TESTO", "TESTO");
//            await Map("HCG", "HCG");
//            await Map("PCT", "PCT");
//            await Map("PSA", "PSA");
//            await Map("AFP", "AFP");
//            await Map("CA125", "CA125");
//            await Map("CA15-3", "CA153");
//            await Map("CA19-9", "CA199");
//            await Map("INS", "INS");
//            await Map("CPEP", "CP");


//        }
//    }
//}

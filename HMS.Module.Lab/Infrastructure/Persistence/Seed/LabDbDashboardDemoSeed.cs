using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HMS.Module.Lab.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// End-to-end demo seeder for the Lab DB.
    /// Idempotent and safe to run many times.
    /// Includes: specimen types, catalog (chem + immuno + CBC),
    /// a small panel, demo orders/items/samples, demo results,
    /// and instrument maps for Roche, Sysmex, Fuji.
    /// </summary>
    public static class LabDbDemoSeed
    {
        private const string SeedUser = "seed:demo";

        public static async Task EnsureAllAsync(
            LabDbContext db, IConfiguration cfg, CancellationToken ct = default)
        {
            await EnsureSpecimenTypesAsync(db, ct);
            await EnsureCatalogAsync(db, ct);
            await EnsurePanelsAsync(db, ct);
            await EnsureDemoOrdersAsync(db, ct);
            await EnsureResultsForAllItemsAsync(db, ct);
            await EnsureInstrumentMapsAsync(db, cfg, ct);
        }

        // ----------------------------------------------------------------------
        // 1) Specimen Types
        // ----------------------------------------------------------------------
        private static async Task EnsureSpecimenTypesAsync(LabDbContext db, CancellationToken ct)
        {
            if (await db.SpecimenTypes.AnyAsync(ct)) return;

            var now = DateTime.UtcNow;
            db.SpecimenTypes.AddRange(
                new mySpecimenType { Code = "SER", Name = "Serum", CreatedAt = now, CreatedBy = SeedUser },
                new mySpecimenType { Code = "PLAS", Name = "Plasma", CreatedAt = now, CreatedBy = SeedUser },
                new mySpecimenType { Code = "WB", Name = "Whole Blood", CreatedAt = now, CreatedBy = SeedUser },
                new mySpecimenType { Code = "CSF", Name = "Cerebrospinal fluid", CreatedAt = now, CreatedBy = SeedUser },
                new mySpecimenType { Code = "UR", Name = "Urine", CreatedAt = now, CreatedBy = SeedUser },
                new mySpecimenType { Code = "STL", Name = "Stool", CreatedAt = now, CreatedBy = SeedUser },
                new mySpecimenType { Code = "SEM", Name = "Semen", CreatedAt = now, CreatedBy = SeedUser }
            );
            await db.SaveChangesAsync(ct);
        }

        // ----------------------------------------------------------------------
        // 2) Catalog (Chem + Immuno + CBC)  — avoids circular FK by saving in two steps
        // ----------------------------------------------------------------------
        private static async Task EnsureCatalogAsync(LabDbContext db, CancellationToken ct)
        {
            var serumId = await db.SpecimenTypes
                .Where(s => s.Code == "SER" && !s.IsDeleted)
                .Select(s => s.SpecimenTypeId)
                .FirstAsync(ct);

            async Task EnsureTest(
                string code, string name, string unit,
                (decimal? lo, decimal? hi)? rr,
                (int? roomH, int? refH, int? frozenD) stab,
                decimal price)
            {
                var now = DateTime.UtcNow;
                var t = await db.LabTests
                    .FirstOrDefaultAsync(x => x.Code == code && !x.IsDeleted, ct);

                if (t is null)
                {
                    // Step 1: create test
                    t = new myLabTest
                    {
                        Code = code,
                        Name = name,
                        Unit = unit,
                        Price = price,
                        TatMinutes = 60,
                        SpecimenTypeId = serumId,
                        StabilityRoomHours = stab.roomH,
                        StabilityRefrigeratedHours = stab.refH,
                        StabilityFrozenDays = stab.frozenD,
                        IsActive = true,
                        CreatedAt = now,
                        CreatedBy = SeedUser
                    };
                    db.LabTests.Add(t);
                    await db.SaveChangesAsync(ct); // get LabTestId

                    // Step 2: create default range separately (no circular graph)
                    if (rr is { } r)
                    {
                        var range = new myReferenceRange
                        {
                            LabTestId = t.LabTestId,
                            RefLow = r.lo,
                            RefHigh = r.hi,
                            CreatedAt = now,
                            CreatedBy = SeedUser
                        };
                        db.ReferenceRanges.Add(range);
                        await db.SaveChangesAsync(ct);

                        t.DefaultReferenceRangeId = range.ReferenceRangeId;
                        await db.SaveChangesAsync(ct);
                    }
                }
                else if ((t.CreatedBy ?? "").StartsWith("seed:"))
                {
                    t.Name = name;
                    t.Unit = unit;
                    t.Price = price;
                    t.SpecimenTypeId = serumId;
                    t.StabilityRoomHours = stab.roomH;
                    t.StabilityRefrigeratedHours = stab.refH;
                    t.StabilityFrozenDays = stab.frozenD;
                    t.IsActive = true;
                    t.UpdatedAt = now;
                    t.UpdatedBy = SeedUser;
                    await db.SaveChangesAsync(ct);

                    if (rr is { } r)
                    {
                        myReferenceRange? range = null;
                        if (t.DefaultReferenceRangeId.HasValue)
                        {
                            range = await db.ReferenceRanges
                                .FirstOrDefaultAsync(x => x.ReferenceRangeId == t.DefaultReferenceRangeId.Value, ct);
                        }
                        if (range is null)
                        {
                            range = new myReferenceRange
                            {
                                LabTestId = t.LabTestId,
                                CreatedAt = now,
                                CreatedBy = SeedUser
                            };
                            db.ReferenceRanges.Add(range);
                        }
                        range.RefLow = r.lo;
                        range.RefHigh = r.hi;
                        range.UpdatedAt = now;
                        range.UpdatedBy = SeedUser;
                        await db.SaveChangesAsync(ct);

                        if (t.DefaultReferenceRangeId != range.ReferenceRangeId)
                        {
                            t.DefaultReferenceRangeId = range.ReferenceRangeId;
                            await db.SaveChangesAsync(ct);
                        }
                    }
                }
            }

            // ---- Chemistry (subset) ----
            await EnsureTest("GLU", "Glucose", "mg/dL", (70, 110), (8, 48, 30), 2.75m);
            await EnsureTest("UREA", "Urea (BUN)", "mg/dL", (15, 45), (8, 48, 30), 3.20m);
            await EnsureTest("CREA", "Creatinine", "mg/dL", (0.6m, 1.2m), (8, 48, 30), 3.00m);
            await EnsureTest("UA", "Uric Acid", "mg/dL", (3.4m, 7.0m), (8, 48, 30), 3.00m);
            await EnsureTest("CHOL", "Cholesterol", "mg/dL", (0, 200), (8, 48, 30), 3.00m);
            await EnsureTest("TG", "Triglycerides", "mg/dL", (0, 150), (8, 48, 30), 3.00m);
            await EnsureTest("HDL", "HDL Cholesterol", "mg/dL", (40, 80), (8, 48, 30), 3.00m);
            await EnsureTest("LDL", "LDL Cholesterol", "mg/dL", (0, 130), (8, 48, 30), 3.00m);
            await EnsureTest("ALT", "ALT (GPT)", "U/L", (0, 40), (8, 48, 30), 3.00m);
            await EnsureTest("AST", "AST (GOT)", "U/L", (0, 40), (8, 48, 30), 3.00m);
            await EnsureTest("ALP", "Alkaline Phosphatase", "U/L", (44, 147), (8, 48, 30), 3.00m);
            await EnsureTest("NA", "Sodium", "mmol/L", (135, 145), (8, 48, 30), 3.00m);
            await EnsureTest("K", "Potassium", "mmol/L", (3.5m, 5.1m), (8, 48, 30), 3.00m);
            await EnsureTest("CL", "Chloride", "mmol/L", (98, 107), (8, 48, 30), 3.00m);

            // ---- Immuno (examples) ----
            await EnsureTest("TSH", "TSH", "µIU/mL", (0.4m, 4.0m), (8, 72, 90), 3.50m);
            await EnsureTest("FT4", "Free T4", "ng/dL", (0.8m, 1.8m), (8, 72, 90), 3.50m);
            await EnsureTest("FT3", "Free T3", "pg/mL", (2.3m, 4.2m), (8, 72, 90), 3.50m);
            await EnsureTest("PRL", "Prolactin", "ng/mL", (4, 23), (8, 72, 90), 3.50m);
            await EnsureTest("PSA", "PSA Total", "ng/mL", (0, 4), (8, 72, 90), 4.00m);

            // ---- CBC basics (Sysmex) ----
            await EnsureTest("WBC", "White Blood Cells", "10^9/L", (4.0m, 10.0m), (null, null, null), 3.00m);
            await EnsureTest("RBC", "Red Blood Cells", "10^12/L", (3.8m, 5.2m), (null, null, null), 3.00m);
            await EnsureTest("HGB", "Hemoglobin", "g/dL", (11.5m, 15.5m), (null, null, null), 3.00m);
            await EnsureTest("HCT", "Hematocrit", "%", (35m, 47m), (null, null, null), 3.00m);
            await EnsureTest("MCV", "MCV", "fL", (78m, 98m), (null, null, null), 3.00m);
            await EnsureTest("MCH", "MCH", "pg", (26m, 34m), (null, null, null), 3.00m);
            await EnsureTest("MCHC", "MCHC", "g/dL", (30m, 35m), (null, null, null), 3.00m);
            await EnsureTest("RDW_CV", "RDW-CV", "%", (null, 14.5m), (null, null, null), 3.00m);
            await EnsureTest("PLT", "Platelets", "10^9/L", (150m, 450m), (null, null, null), 3.00m);
            await EnsureTest("MPV", "MPV", "fL", (7.5m, 12.5m), (null, null, null), 3.00m);
        }

        // ----------------------------------------------------------------------
        // 3) Small panel (EL = NA + K)
        // ----------------------------------------------------------------------
        private static async Task EnsurePanelsAsync(LabDbContext db, CancellationToken ct)
        {
            if (await db.LabPanels.AnyAsync(p => p.Code == "EL" && !p.IsDeleted, ct)) return;

            var now = DateTime.UtcNow;
            var naId = await db.LabTests.Where(t => t.Code == "NA").Select(t => t.LabTestId).FirstOrDefaultAsync(ct);
            var kId = await db.LabTests.Where(t => t.Code == "K").Select(t => t.LabTestId).FirstOrDefaultAsync(ct);
            if (naId == 0 || kId == 0) return;

            var panel = new myLabPanel
            {
                Code = "EL",
                Name = "Electrolytes",
                IsActive = true,
                CreatedAt = now,
                CreatedBy = SeedUser
            };
            db.LabPanels.Add(panel);
            await db.SaveChangesAsync(ct);

            db.LabPanelItems.AddRange(
                new myLabPanelItem { LabPanelId = panel.LabPanelId, LabTestId = naId, SortOrder = 1, CreatedAt = now, CreatedBy = SeedUser },
                new myLabPanelItem { LabPanelId = panel.LabPanelId, LabTestId = kId, SortOrder = 2, CreatedAt = now, CreatedBy = SeedUser }
            );
            await db.SaveChangesAsync(ct);
        }

        // ----------------------------------------------------------------------
        // 4) Demo order + items + sample
        // ----------------------------------------------------------------------
        private static async Task EnsureDemoOrdersAsync(LabDbContext db, CancellationToken ct)
        {
            async Task<long> EnsureReq(string orderNo, IEnumerable<string> codes, string accession)
            {
                var now = DateTime.UtcNow;
                var reqId = await db.LabRequests
                    .Where(r => r.OrderNo == orderNo && !r.IsDeleted)
                    .Select(r => r.LabRequestId).FirstOrDefaultAsync(ct);

                if (reqId == 0)
                {
                    var req = new myLabRequest
                    {
                        PatientId = 1,
                        OrderNo = orderNo,
                        Status = LabRequestStatus.Requested,
                        CreatedAt = now,
                        CreatedBy = SeedUser
                    };
                    db.LabRequests.Add(req);
                    await db.SaveChangesAsync(ct);
                    reqId = req.LabRequestId;
                }

                var wanted = codes.Select(c => c.Trim().ToUpperInvariant()).Distinct().ToArray();
                var tests = await db.LabTests.Where(t => wanted.Contains(t.Code)).ToListAsync(ct);

                foreach (var t in tests)
                {
                    var exists = await db.LabRequestItems
                        .AnyAsync(i => i.LabRequestId == reqId && i.LabTestId == t.LabTestId && !i.IsDeleted, ct);
                    if (!exists)
                    {
                        db.LabRequestItems.Add(new myLabRequestItem
                        {
                            LabRequestId = reqId,
                            LabTestId = t.LabTestId,
                            LabTestCode = t.Code,
                            LabTestName = t.Name,
                            LabTestUnit = t.Unit,
                            CreatedAt = now,
                            CreatedBy = SeedUser
                        });
                    }
                }
                await db.SaveChangesAsync(ct);

                var hasSample = await db.LabSamples
                    .AnyAsync(s => s.LabRequestId == reqId && s.AccessionNumber == accession && !s.IsDeleted, ct);
                if (!hasSample)
                {
                    db.LabSamples.Add(new myLabSample
                    {
                        LabRequestId = reqId,
                        AccessionNumber = accession,
                        Status = LabSampleStatus.Collected,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = SeedUser
                    });
                    await db.SaveChangesAsync(ct);
                }
                return reqId;
            }

            await EnsureReq("ORD-SEED-001", new[] { "GLU", "UREA" }, "ACC0001");
            await EnsureReq("ORD-CBC-DEMO-001", new[] { "WBC", "RBC", "HGB", "HCT", "MCV", "MCH", "MCHC", "RDW_CV", "PLT", "MPV" }, "ACC-CBC-0001");
        }

        // ----------------------------------------------------------------------
        // 5) Demo results for items that don't have one yet
        // ----------------------------------------------------------------------
        private static async Task EnsureResultsForAllItemsAsync(LabDbContext db, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var existing = new HashSet<long>(await db.LabResults.AsNoTracking()
                .Select(r => r.LabRequestItemId).ToListAsync(ct));

            var items = await db.LabRequestItems
                .Include(i => i.LabTest)
                .Where(i => !i.IsDeleted && !existing.Contains(i.LabRequestItemId))
                .ToListAsync(ct);
            if (items.Count == 0) return;

            var accByReq = await db.LabSamples.AsNoTracking()
                .GroupBy(s => s.LabRequestId)
                .Select(g => new { g.Key, Accession = g.OrderByDescending(x => x.CreatedAt).Select(x => x.AccessionNumber).FirstOrDefault() })
                .ToDictionaryAsync(x => x.Key, x => x.Accession, ct);

            foreach (var it in items)
            {
                var t = it.LabTest!;
                var val = t.Code switch
                {
                    "GLU" => "104",
                    "UREA" => "31",
                    "NA" => "139",
                    "K" => "4.2",
                    "WBC" => "9.56",
                    "RBC" => "4.68",
                    "HGB" => "10.4",
                    "HCT" => "33.4",
                    "MCV" => "71.5",
                    "MCH" => "22.3",
                    "MCHC" => "31.3",
                    "RDW_CV" => "16.6",
                    "PLT" => "336",
                    "MPV" => "9.1",
                    _ => "1"
                };
                accByReq.TryGetValue(it.LabRequestId, out var acc);

                db.LabResults.Add(new myLabResult
                {
                    LabRequestId = it.LabRequestId,
                    LabRequestItemId = it.LabRequestItemId,
                    AccessionNumber = acc,
                    LabTestId = t.LabTestId,
                    LabTestCode = t.Code,
                    LabTestName = t.Name,
                    Value = val,
                    Unit = it.LabTestUnit ?? t.Unit,
                    RefLow = t.DefaultReferenceRange?.RefLow,
                    RefHigh = t.DefaultReferenceRange?.RefHigh,
                    Status = LabResultStatus.Final,
                    CreatedAt = now,
                    CreatedBy = SeedUser,
                    UpdatedAt = now,
                    UpdatedBy = SeedUser
                });
            }
            await db.SaveChangesAsync(ct);
        }

        // ----------------------------------------------------------------------
        // 6) Instrument Maps (Roche + Sysmex + Fuji)
        // ----------------------------------------------------------------------
        // in LabDbDemoSeed.cs
        private static async Task EnsureInstrumentMapsAsync(LabDbContext db, IConfiguration cfg, CancellationToken ct)
        {
            async Task MapDevice(long deviceId, string hostCode, string labCode, string createdBy)
            {
                var t = await db.LabTests
                    .Where(x => x.Code == labCode && !x.IsDeleted)
                    .Select(x => new { x.LabTestId, x.Code })
                    .FirstOrDefaultAsync(ct);
                if (t is null) return;

                var exists = await db.InstrumentTestMaps.AnyAsync(m =>
                    m.DeviceId == deviceId &&
                    m.InstrumentTestCode == hostCode &&
                    !m.IsDeleted, ct);

                if (!exists)
                {
                    db.InstrumentTestMaps.Add(new myInstrumentTestMap
                    {
                        DeviceId = deviceId,
                        InstrumentTestCode = hostCode,
                        LabTestId = t.LabTestId,
                        LabTestCode = t.Code,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = createdBy
                    });
                    await db.SaveChangesAsync(ct);
                }
            }

            // Helper to read a numeric ID from config; returns null if not present
            static long? GetId(IConfiguration c, string key)
                => long.TryParse(c[key], out var id) && id > 0 ? id : (long?)null;

            // ---------------- Roche ----------------
            var rocheId = GetId(cfg, "Communication:Transport:Seed:DeviceId")
                        ?? GetId(cfg, "Communication:Seed:DeviceId");
            if (rocheId is > 0)
            {
                async Task MapRoche(string host, string lab) => await MapDevice(rocheId.Value, host, lab, "seed:roche");

                await MapRoche("717", "GLU");
                await MapRoche("989", "NA");
                await MapRoche("990", "K");
                await MapRoche("991", "CL");
                await MapRoche("690", "CREA");
                await MapRoche("781", "TG");
                await MapRoche("552", "LDL");
                await MapRoche("454", "HDL");
                await MapRoche("798", "CHOL");
                await MapRoche("735", "DBIL");
                await MapRoche("734", "TBIL");
                await MapRoche("685", "ALT");
                await MapRoche("587", "AST");
                await MapRoche("962", "CA");
                await MapRoche("656", "PHOS");
                await MapRoche("779", "UIBC");
                await MapRoche("418", "UREA");

                // e411/e601 placeholders – replace with your actual codes if different
                await MapRoche("001", "TSH");
                await MapRoche("002", "FT4");
                await MapRoche("003", "FT3");
                await MapRoche("010", "PRL");
                await MapRoche("060", "PSA");
            }

            // ---------------- Sysmex XP-300 ----------------
            var sysmexId = GetId(cfg, "Communication:Transport:Seed:SysmexDeviceId")
                        ?? GetId(cfg, "Communication:Seed:SysmexDeviceId");
            if (sysmexId is > 0)
            {
                async Task MapSys(string host, string lab) => await MapDevice(sysmexId.Value, host, lab, "seed:sysmex");

                await MapSys("WBC", "WBC");
                await MapSys("RBC", "RBC");
                await MapSys("HGB", "HGB");
                await MapSys("HCT", "HCT");
                await MapSys("MCV", "MCV");
                await MapSys("MCH", "MCH");
                await MapSys("MCHC", "MCHC");
                await MapSys("PLT", "PLT");
                await MapSys("MPV", "MPV");
                await MapSys("RDW-CV", "RDW_CV"); // some models send RDW-CV with a dash
            }

            // ---------------- Fuji Dri-Chem NX500 ----------------
            var fujiId = GetId(cfg, "Communication:Transport:Seed:FujiDeviceId")
                      ?? GetId(cfg, "Communication:Seed:FujiDeviceId");
            if (fujiId is > 0)
            {
                async Task MapFuji(string host, string lab) => await MapDevice(fujiId.Value, host, lab, "seed:fuji");

                await MapFuji("GLU", "GLU");
                await MapFuji("BUN", "UREA");
                await MapFuji("CRE", "CREA");
                await MapFuji("UA", "UA");
                await MapFuji("TC", "CHOL");
                await MapFuji("TG", "TG");
                await MapFuji("TP", "TP");
                await MapFuji("ALB", "ALB");
                await MapFuji("TBIL", "TBIL");
                await MapFuji("AST", "AST");
                await MapFuji("ALT", "ALT");
                await MapFuji("ALP", "ALP");
                await MapFuji("GGT", "GGT");
                await MapFuji("AMY", "AMY");
                await MapFuji("LDH", "LDH");
                await MapFuji("Ca", "CA");
                await MapFuji("IP", "PHOS");
                await MapFuji("Mg", "MG");
                await MapFuji("CRP", "CRP");
            }
        }

    }
}

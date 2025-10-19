using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints.Requests
{
    public static class EditRequestEndpoints
    {
        public static IEndpointRouteBuilder MapLabEditRequestEndpoints(this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/api/v1/lab/requests").WithTags("Laboratory");

            // GET edit data
            g.MapGet("/{id:long}/edit-data", async (
                long id,
                LabDbContext db,
                CancellationToken ct) =>
            {
                var r = await db.LabRequests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.LabRequestId == id && !x.IsDeleted, ct);
                if (r is null) return Results.NotFound();

                var hasAnyResult = await db.LabResults
                    .AsNoTracking()
                    .AnyAsync(x => x.LabRequestId == id && !x.IsDeleted, ct);

                var selectedIds = await db.LabRequestItems
                    .AsNoTracking()
                    .Where(i => i.LabRequestId == id && !i.IsDeleted)
                    .Select(i => i.LabTestId)
                    .ToListAsync(ct);

                var tests = await db.LabTests
                    .AsNoTracking()
                    .OrderBy(t => t.Code)
                    .Select(t => new TestRow(
                        t.LabTestId, t.Code, t.Name, t.Unit, t.Price,
                        selectedIds.Contains(t.LabTestId)))
                    .ToListAsync(ct);

                var head = new EditHeadDto(
                    r.LabRequestId, r.OrderNo,
                    r.PatientDisplay, r.PatientId, r.LabPatientId,
                    r.DoctorDisplay, r.DoctorId, r.LabDoctorId,
                    r.Source, r.CreatedAt,
                    hasAnyResult);

                return Results.Ok(new EditDataDto(head, tests));
            })
            .WithName("Lab_EditRequest_Data_v1");

            // POST apply changes (only if NO results)
            // ...using directives unchanged...

            // inside MapLabEditRequestEndpoints(...)
            g.MapPost("/{id:long}/apply", async (
                long id,
                [FromBody] ApplyDto dto,
                LabDbContext db,
                CancellationToken ct) =>
            {
                if (id != dto.LabRequestId) return Results.BadRequest("Mismatched id.");

                var r = await db.LabRequests
                    .Include(x => x.Items.Where(i => !i.IsDeleted))
                    .Include(x => x.Samples.Where(s => !s.IsDeleted)) // to get accession back
                    .FirstOrDefaultAsync(x => x.LabRequestId == id && !x.IsDeleted, ct);

                if (r is null) return Results.NotFound();

                var hasAnyResult = await db.LabResults
                    .AnyAsync(x => x.LabRequestId == id && !x.IsDeleted, ct);
                if (hasAnyResult)
                    return Results.Conflict("Cannot edit: results already exist.");

                // Update doctor
                r.DoctorId = dto.DoctorId;
                r.LabDoctorId = dto.LabDoctorId;
                r.DoctorDisplay = string.IsNullOrWhiteSpace(dto.DoctorName) ? r.DoctorDisplay : dto.DoctorName!.Trim();

                // Recompute item set
                var wanted = dto.TestIds?.Distinct().ToHashSet() ?? new HashSet<long>();
                var current = r.Items.Select(i => i.LabTestId).ToHashSet();
                // ...
                var toAdd = wanted.Except(current).ToList();
                var toDel = current.Except(wanted).ToList();

                // 🔽 ADD THIS block before creating new rows
                if (toAdd.Count > 0)
                {
                    // revive soft-deleted if same LabTestId existed before
                    var revive = r.Items.Where(i => i.IsDeleted && toAdd.Contains(i.LabTestId)).ToList();
                    foreach (var i in revive)
                    {
                        i.IsDeleted = false;
                        i.CreatedAt = DateTime.UtcNow;
                        i.UpdatedBy = "edit";
                    }
                    // remove revived from toAdd (so we don't insert duplicates)
                    toAdd = toAdd.Except(revive.Select(x => x.LabTestId)).ToList();
                }

                if (toAdd.Count > 0)
                {
                    var addTests = await db.LabTests
                        .Where(t => toAdd.Contains(t.LabTestId))
                        .Select(t => new { t.LabTestId, t.Code, t.Name, t.Unit, t.Price })
                        .ToListAsync(ct);

                    foreach (var t in addTests)
                    {
                        // final guard: if a non-deleted row with same code exists, skip
                        if (r.Items.Any(i => !i.IsDeleted && i.LabTestId == t.LabTestId)) continue;

                        r.Items.Add(new myLabRequestItem
                        {
                            LabRequestId = r.LabRequestId,
                            LabTestId = t.LabTestId,
                            LabTestCode = t.Code,
                            LabTestName = t.Name,
                            LabTestUnit = t.Unit,
                            LabTestPrice = t.Price,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "edit"
                        });
                    }
                }
                // ...


                r.UpdatedAt = DateTime.UtcNow;
                r.UpdatedBy = "edit";
                await db.SaveChangesAsync(ct);

                // ... you already updated the request + items above this line
                //await db.SaveChangesAsync(ct);

                // First accession on this request (if any)
                var firstAccession = await db.LabSamples
                    .Where(s => s.LabRequestId == r.LabRequestId)
                    .OrderBy(s => s.LabSampleId)
                    .Select(s => s.AccessionNumber)
                    .FirstOrDefaultAsync(ct);

                // If test set changed, mark all linked samples as "needs reprint"
                var testsChanged = (toAdd.Count + toDel.Count) > 0;
                if (testsChanged && !string.IsNullOrEmpty(firstAccession))
                {
                    var samples = await db.LabSamples
                        .Where(s => s.LabRequestId == r.LabRequestId)
                        .ToListAsync(ct);

                    foreach (var s in samples)
                    {
                        s.LabelPrinted = false;          // will be flipped to true when label page opens
                        s.UpdatedAt = DateTime.UtcNow;
                        s.UpdatedBy = "edit";
                    }
                    await db.SaveChangesAsync(ct);
                }

                return Results.Ok(new
                {
                    ok = true,
                    testsChanged,
                    accession = firstAccession
                });

            })
            .WithName("Lab_EditRequest_Apply_v1");


            return app;
        }
    }
}
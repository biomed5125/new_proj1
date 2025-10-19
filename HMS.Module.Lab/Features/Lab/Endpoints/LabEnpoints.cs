using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.SharedKernel.Ids;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Linq;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class LabEndpoints
{
    public static IEndpointRouteBuilder MapLabEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab").WithTags("Lab v1");

        g.MapGet("/requests", async (LabDbContext db, CancellationToken ct) =>
        {
            var items = await db.LabRequests.AsNoTracking()
                .OrderByDescending(x => x.LabRequestId)
                .Select(x => new { x.LabRequestId, x.PatientId, x.OrderNo, x.Status, x.Priority, Items = x.Items.Count })
                .ToListAsync(ct);
            return Results.Ok(items);
        });

        g.MapPost("/requests", async (CreateLabRequestDto dto,
                                      IValidator<CreateLabRequestDto> validator,
                                      LabDbContext db,
                                      [FromKeyedServices("lab")] IBusinessIdGenerator ids,
                                      CancellationToken ct) =>
        {
            var val = await validator.ValidateAsync(dto, ct);
            if (!val.IsValid)
            {
                var problem = val.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Results.ValidationProblem(problem);
            }

            var now = DateTime.UtcNow;
            var req = new myLabRequest
            {
                PatientId = dto.PatientId,
                AdmissionId = dto.AdmissionId,
                DoctorId = dto.DoctorId,
                Priority = dto.Priority,
                Notes = dto.Notes,
                OrderNo = ids.NewLabOrderNo(now),
                CreatedAt = now,
                CreatedBy = "api"
            };

            // add items
            foreach (var testId in dto.TestIds.Distinct())
                req.Items.Add(new myLabRequestItem { LabTestId = testId, CreatedAt = now, CreatedBy = "api" });

            db.LabRequests.Add(req);

            // create a sample with an accession number
            var sample = new myLabSample
            {
                LabRequestId = req.LabRequestId, // will be set after SaveChanges
                AccessionNumber = ids.NewAccessionNumber(now),
                Status = Models.Enums.LabSampleStatus.Collected,
                CreatedAt = now,
                CreatedBy = "api"
            };
            db.LabSamples.Add(sample);

            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v1/lab/requests/{req.LabRequestId}", new { req.LabRequestId, req.OrderNo });
        });

        g.MapPost("/results", async (EnterResultDto dto,
                                     IValidator<EnterResultDto> validator,
                                     LabDbContext db,
                                     CancellationToken ct) =>
        {
            var val = await validator.ValidateAsync(dto, ct);
            if (!val.IsValid)
            {
                var problem = val.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Results.ValidationProblem(problem);
            }

            var item = await db.LabRequestItems.AsNoTracking()
                       .FirstOrDefaultAsync(x => x.LabRequestItemId == dto.LabRequestItemId, ct);
            if (item is null) return Results.NotFound("LabRequestItem not found.");

            var res = new myLabResult
            {
                LabRequestId = item.LabRequestId,
                LabRequestItemId = item.LabRequestItemId,
                Value = dto.Value,
                Unit = dto.Unit,
                RefLow = dto.RefLow,
                RefHigh = dto.RefHigh,
                Flag = dto.Flag,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "api"
            };

            db.LabResults.Add(res);
            await db.SaveChangesAsync(ct);
            return Results.Ok(new { res.LabResultId });
        });

        return app;
    }
}



//// Features/Lab/Endpoints/LabEndpoints.cs
//using HMS.Module.Lab.Features.Lab.Models.Dtos;
//using HMS.Module.Lab.Features.Lab.Models.Entities;
//using HMS.Module.Lab.Features.Lab.Models.Enums;
//using HMS.Module.Lab.Infrastructure.Persistence;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Module.Lab.Features.Lab.Endpoints;

//public static class LabEndpoints
//{
//    public static IEndpointRouteBuilder MapLabEndpoints(this IEndpointRouteBuilder app)
//    {
//        var g = app.MapGroup("/api/v1/lab").WithTags("Lab");

//        // ---- Tests ----
//        g.MapGet("/tests", async (LabDbContext db, CancellationToken ct) =>
//        {
//            var tests = await db.LabTests
//                .Where(t => !t.IsDeleted && t.IsActive)
//                .OrderBy(t => t.Code)
//                .Select(t => new
//                {
//                    t.LabTestId,
//                    t.Code,
//                    t.Name,
//                    t.SpecimenType,
//                    t.TatMinutes,
//                    t.IsActive
//                })
//                .ToListAsync(ct);

//            return Results.Ok(tests);
//        });

//        // ---- Requests ----
//        g.MapGet("/requests", async (LabDbContext db, CancellationToken ct) =>
//        {
//            var list = await db.LabRequests
//                .Where(r => !r.IsDeleted)
//                .OrderByDescending(r => r.CreatedAt)
//                .Select(r => new
//                {
//                    r.LabRequestId,
//                    r.OrderNo,
//                    r.PatientId,
//                    r.AdmissionId,
//                    r.DoctorId,
//                    r.Priority,
//                    r.Status,
//                    r.CreatedAt
//                })
//                .ToListAsync(ct);

//            return Results.Ok(list);
//        });

//        g.MapGet("/requests/{id:long}", async (long id, LabDbContext db, CancellationToken ct) =>
//        {
//            var r = await db.LabRequests
//                .Where(x => x.LabRequestId == id && !x.IsDeleted)
//                .Select(x => new
//                {
//                    x.LabRequestId,
//                    x.OrderNo,
//                    x.PatientId,
//                    x.AdmissionId,
//                    x.DoctorId,
//                    x.Priority,
//                    x.Status,
//                    x.Notes,
//                    x.CreatedAt,
//                    Items = x.Items.Where(i => !i.IsDeleted)
//                                   .Select(i => new { i.LabRequestItemId, i.LabTestId }),
//                    Sample = db.LabSamples
//                               .Where(s => s.LabRequestId == x.LabRequestId && !s.IsDeleted)
//                               .Select(s => new { s.LabSampleId, s.AccessionNumber, s.Status, s.CollectedAtUtc, s.ReceivedAtUtc })
//                               .FirstOrDefault(),
//                    Results = db.LabResults
//                                .Where(res => res.LabRequestId == x.LabRequestId && !res.IsDeleted)
//                                .Select(res => new { res.LabResultId, res.LabRequestItemId, res.Value, res.Unit, res.RefLow, res.RefHigh, res.Flag, res.Status })
//                                .ToList()
//                })
//                .FirstOrDefaultAsync(ct);

//            return r is null ? Results.NotFound() : Results.Ok(r);
//        });


//        g.MapPost("/requests", async (CreateRequestDto dto, LabDbContext db, CancellationToken ct) =>
//        {
//            var now = DateTime.UtcNow;
//            string NewOrderNo(DateTime t) => $"LR{t:yyyyMMddHHmmssfff}";

//            var req = new myLabRequest
//            {
//                PatientId = dto.PatientId,
//                AdmissionId = dto.AdmissionId,
//                DoctorId = dto.DoctorId,
//                Priority = dto.Priority,
//                Status = LabRequestStatus.Requested,
//                Notes = dto.Notes,
//                OrderNo = NewOrderNo(now),
//                CreatedAt = now,
//                CreatedBy = "api"
//            };
//            db.LabRequests.Add(req);
//            await db.SaveChangesAsync(ct);

//            foreach (var testId in dto.LabTestIds.Distinct())
//            {
//                db.LabRequestItems.Add(new myLabRequestItem
//                {
//                    LabRequestId = req.LabRequestId,
//                    LabTestId = testId,
//                    CreatedAt = now,
//                    CreatedBy = "api"
//                });
//            }
//            await db.SaveChangesAsync(ct);

//            return Results.Created($"/api/v1/lab/requests/{req.LabRequestId}", new { req.LabRequestId, req.OrderNo });
//        });

//        g.MapPost("/requests/{id:long}/collect", async (long id, CollectDto dto, LabDbContext db, CancellationToken ct) =>
//        {
//            var req = await db.LabRequests.FirstOrDefaultAsync(r => r.LabRequestId == id && !r.IsDeleted, ct);
//            if (req is null) return Results.NotFound();

//            string NewAccession(DateTime t) => $"AC{t:yyyyMMddHHmmss}";

//            var sample = new myLabSample
//            {
//                LabRequestId = req.LabRequestId,
//                AccessionNumber = NewAccession(dto.CollectedAtUtc),
//                Status = LabSampleStatus.Collected,
//                CollectedAtUtc = dto.CollectedAtUtc,
//                CollectedBy = "api",
//                CreatedAt = DateTime.UtcNow,
//                CreatedBy = "api"
//            };
//            db.LabSamples.Add(sample);
//            await db.SaveChangesAsync(ct);

//            return Results.Ok(new { sample.LabSampleId, sample.AccessionNumber, sample.Status });
//        });

//        g.MapPost("/requests/{id:long}/receive", async (long id, ReceiveDto dto, LabDbContext db, CancellationToken ct) =>
//        {
//            var sample = await db.LabSamples
//                .Where(s => s.LabRequestId == id && !s.IsDeleted)
//                .OrderByDescending(s => s.CreatedAt)
//                .FirstOrDefaultAsync(ct);

//            if (sample is null) return Results.NotFound();

//            sample.Status = LabSampleStatus.Received;
//            sample.ReceivedAtUtc = dto.ReceivedAtUtc;
//            sample.ReceivedBy = "api";
//            sample.UpdatedAt = DateTime.UtcNow; sample.UpdatedBy = "api";
//            await db.SaveChangesAsync(ct);

//            return Results.Ok(new { sample.LabSampleId, sample.Status, sample.ReceivedAtUtc });
//        });

//        g.MapPost("/requests/{id:long}/results/enter", async (long id, List<EnterResultDto> body, LabDbContext db, CancellationToken ct) =>
//        {
//            var now = DateTime.UtcNow;

//            foreach (var r in body)
//            {
//                db.LabResults.Add(new myLabResult
//                {
//                    LabRequestId = id,
//                    LabRequestItemId = r.LabRequestItemId,
//                    Value = r.Value,
//                    Unit = r.Unit,
//                    RefLow = r.RefLow,
//                    RefHigh = r.RefHigh,
//                    Flag = r.Flag ?? "N",
//                    Status = LabResultStatus.Entered,
//                    CreatedAt = now,
//                    CreatedBy = "api"
//                });
//            }
//            await db.SaveChangesAsync(ct);
//            return Results.Ok();
//        });

//        g.MapPost("/requests/{id:long}/approve", async (long id, LabDbContext db, CancellationToken ct) =>
//        {
//            var now = DateTime.UtcNow;
//            var results = await db.LabResults
//                                  .Where(r => r.LabRequestId == id && !r.IsDeleted)
//                                  .ToListAsync(ct);
//            if (results.Count == 0) return Results.BadRequest(new { error = "No results to approve." });

//            foreach (var r in results)
//            {
//                r.Status = LabResultStatus.Approved;
//                r.ApprovedAtUtc = now;
//                r.ApprovedByDoctorId = null; // set if you have the doctor id
//                r.UpdatedAt = now; r.UpdatedBy = "api";
//            }
//            var req = await db.LabRequests.FirstAsync(r => r.LabRequestId == id, ct);
//            req.Status = LabRequestStatus.Approved;
//            req.UpdatedAt = now; req.UpdatedBy = "api";

//            await db.SaveChangesAsync(ct);
//            return Results.Ok();
//        });

//        return app;
//    }
//}

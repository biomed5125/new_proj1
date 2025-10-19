using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.SharedKernel.Ids;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class LisManualOrderEndpoints
{
    public static IEndpointRouteBuilder MapLisManualOrders(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/lis/orders").WithTags("LIS");

        g.MapPost("/manual", async (
            [FromBody] ManualOrderCreateDto dto,
            LabDbContext db,
            IBusinessIdGenerator idgen,
            CancellationToken ct) =>
        {
            if (dto.TestIds is null || dto.TestIds.Count == 0)
                return Results.BadRequest("At least one test is required.");

            var patient = await db.LabPatients.AsNoTracking()
                              .FirstOrDefaultAsync(x => x.LabPatientId == dto.LabPatientId, ct);
            if (patient is null) return Results.BadRequest("Unknown patient.");

            myLabDoctor? doctor = null;
            if (dto.LabDoctorId is not null)
                doctor = await db.LabDoctors.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.LabDoctorId == dto.LabDoctorId, ct);

            var now = DateTime.UtcNow;
            var orderNo = idgen.NewLabOrderNo(now); // LRyyyyMMddHHmmssfff

            var req = new myLabRequest
            {
                OrderNo = orderNo,
                Status = LabRequestStatus.Requested,
                LabPatientId = patient.LabPatientId,
                LabDoctorId = doctor?.LabDoctorId,
                PatientDisplay = patient.FullName,
                DoctorDisplay = doctor?.FullName,
                CreatedAt = now,
                CreatedBy = "lis"
            };
            db.LabRequests.Add(req);
            await db.SaveChangesAsync(ct); // get LabRequestId

            // resolve tests
            var tests = await db.LabTests.AsNoTracking()
                         .Where(t => dto.TestIds.Contains(t.LabTestId))
                         .Select(t => new { t.LabTestId, t.Code })
                         .ToListAsync(ct);

            foreach (var t in tests)
            {
                db.LabRequestItems.Add(new myLabRequestItem
                {
                    LabRequestId = req.LabRequestId,
                    LabTestId = t.LabTestId,
                    LabTestCode = t.Code,
                    CreatedAt = now,
                    CreatedBy = "lis"
                });
            }
            await db.SaveChangesAsync(ct);

            // create sample + accession
            var accession = idgen.NewAccessionNumber(now); // ACC-YYYYMMDD-HHMMSS-XYZ (your generator)
            var sample = new myLabSample
            {
                LabRequestId = req.LabRequestId,
                AccessionNumber = accession,
                Status = LabSampleStatus.Collected, // collected at entry
                CreatedAt = now,
                CreatedBy = "lis"
            };
            db.LabSamples.Add(sample);
            await db.SaveChangesAsync(ct);

            return Results.Ok(new ManualOrderCreateResponse(
                req.LabRequestId, orderNo, sample.LabSampleId, accession));
        });

        return app;
    }
}

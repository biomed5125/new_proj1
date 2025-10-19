// Features/Lab/Endpoints/WalkInOrderEndpoints.cs
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

public static class WalkInOrderEndpoints
{
    public static IEndpointRouteBuilder MapWalkInOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/orders").WithTags("Laboratory");

        g.MapPost("/walkin", async (
            [FromBody] CreateWalkInOrderDto dto,
            [FromServices] LabDbContext db,
            CancellationToken ct) =>
        {
            // ----- resolve Patient/Doctor display text (IDs preferred) -----
            string? patientDisplay = null, doctorDisplay = null;

            if (dto.PatientId is > 0)
            {
                var p = await db.LabPatients.AsNoTracking()
                    .Where(x => x.LabPatientId == dto.PatientId)
                    .Select(x => new { x.FullName, x.Sex, x.DateOfBirth })
                    .SingleOrDefaultAsync(ct);
                if (p is null) return Results.BadRequest("Unknown PatientId.");
                patientDisplay = $"{p.FullName}{(p.Sex is null ? "" : $" / {p.Sex}")}{(p.DateOfBirth is null ? "" : $" / {p.DateOfBirth:yyyy-MM-dd}")}";
            }
            else if (!string.IsNullOrWhiteSpace(dto.PatientName))
            {
                patientDisplay = $"{dto.PatientName!.Trim()}{(dto.Sex is null ? "" : $" / {dto.Sex}")}{(dto.Dob is null ? "" : $" / {dto.Dob:yyyy-MM-dd}")}";
            }

            if (dto.DoctorId is > 0)
            {
                var d = await db.LabDoctors.AsNoTracking()
                    .Where(x => x.LabDoctorId == dto.DoctorId)
                    .Select(x => x.FullName)
                    .SingleOrDefaultAsync(ct);
                if (d is null) return Results.BadRequest("Unknown DoctorId.");
                doctorDisplay = d;
            }
            else if (!string.IsNullOrWhiteSpace(dto.DoctorName))
            {
                doctorDisplay = dto.DoctorName!.Trim();
            }

            if (dto.TestIds is null || dto.TestIds.Count == 0)
                return Results.BadRequest("No tests selected.");

            // ----- create request -----
            string orderNo = BuildOrderNo("ORD"); // your helper
            var req = new myLabRequest
            {
                OrderNo = orderNo,
                PatientId = dto.PatientId ?? 0,
                DoctorId = dto.DoctorId,
                PatientDisplay = patientDisplay,
                DoctorDisplay = doctorDisplay,
                Status = LabRequestStatus.Requested,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "ui"
            };

            db.LabRequests.Add(req);
            await db.SaveChangesAsync(ct); // to get LabRequestId

            // ----- add items from selected tests -----
            var testRows = await db.LabTests
                .Where(t => dto.TestIds.Contains(t.LabTestId))
                .ToListAsync(ct);

            if (testRows.Count == 0)
                return Results.BadRequest("Selected TestIds not found.");

            foreach (var t in testRows)
            {
                db.LabRequestItems.Add(new myLabRequestItem
                {
                    LabRequestId = req.LabRequestId,
                    LabTestId = t.LabTestId,
                    LabTestCode = t.Code,
                    LabTestName = t.Name,
                    LabTestUnit = t.Unit,
                    LabTestPrice = t.Price,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "ui"
                });
            }
            await db.SaveChangesAsync(ct);

            // ----- create sample -----
            string accession = BuildAccession("ACC"); // your helper
            var sample = new myLabSample
            {
                LabRequestId = req.LabRequestId,
                AccessionNumber = accession,
                Status = dto.CollectNow ? LabSampleStatus.Collected : LabSampleStatus.Received,
                CollectedAtUtc = dto.CollectNow ? DateTime.UtcNow : null,
                CollectedBy = dto.CollectNow ? "walkin" : null,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "ui"
            };
            db.LabSamples.Add(sample);
            await db.SaveChangesAsync(ct);

            // reply
            return Results.Ok(new WalkInCreatedDto(
                req.LabRequestId, orderNo, sample.LabSampleId, accession));
        });

        return app;
    }

    // helpers you already have somewhere
    private static string BuildOrderNo(string prefix) => $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    private static string BuildAccession(string prefix) => $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Random.Shared.Next(100, 999)}";
}

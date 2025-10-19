// Endpoints/LabOrdersEndpoints.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using static HMS.Module.Lab.Features.Lab.Endpoints.LabOrdersEndpoints.CreateWalkInOrderDto;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class LabOrdersEndpoints
{
    // Unified DTO (IDs preferred; free text fallback) + inline Pre-analytical
    public sealed record CreateWalkInOrderDto(
    long? PatientId,       // OR provide PatientName
    string? PatientName,   // free-text fallback
    string? Sex,
    DateTime? Dob,
    long? DoctorId,        // OR provide DoctorName
    string? DoctorName,
    List<long> TestIds,
    bool CollectNow,
    PreDto? Pre)
    {
        public sealed record PreDto(
       bool IsDiabetic,
       bool TookAntibioticLast3Days,
       int? FastingHours,
       string? ThyroidStatus,
       string? Notes);
    }

    public sealed record WalkInCreatedDto(
        long LabRequestId,
        string OrderNo,
        long LabSampleId,
        string AccessionNumber
    );
    public sealed record CreateWalkInOrderResponse(
        long LabRequestId,
        string OrderNo,
        string? Accession
    );
    public static IEndpointRouteBuilder MapLabOrders(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/orders").WithTags("Laboratory");

        // inside MapLabOrders(...)
        g.MapPost("/walkin", async (
     [FromBody] CreateWalkInOrderDto dto,      // see JS below for shape
     LabDbContext db,
     CancellationToken ct) =>
        {
            // ---- basic test validation
            if (dto.TestIds is null || dto.TestIds.Count == 0)
                return Results.BadRequest("Select at least one test.");

            var tests = await db.LabTests.AsNoTracking()
                .Where(t => dto.TestIds.Contains(t.LabTestId))
                .Select(t => new { t.LabTestId, t.Code, t.Name, t.Unit, t.Price })
                .ToListAsync(ct);
            if (tests.Count != dto.TestIds.Distinct().Count())
                return Results.BadRequest("One or more LabTestIds are invalid.");

            // ---- resolve patient display and ids
            string? patientDisplay = null;
            long? patientId = dto.PatientId;
            long? labPatientId = null;

            if (dto.PatientId is > 0)
            {
                var p = await db.LabPatients.AsNoTracking()
                    .Where(x => x.LabPatientId == dto.PatientId)
                    .Select(x => new { x.LabPatientId, x.FullName, x.Sex, x.DateOfBirth, x.Mrn })
                    .SingleOrDefaultAsync(ct);
                if (p is null) return Results.BadRequest("Unknown PatientId.");
                patientDisplay = $"{p.FullName}{(p.Sex is null ? "" : $" / {p.Sex}")}{(p.DateOfBirth is null ? "" : $" / {p.DateOfBirth:yyyy-MM-dd}")}";
                labPatientId = p.LabPatientId; // LIS-local link
            }
            else if (!string.IsNullOrWhiteSpace(dto.PatientName))
            {
                patientDisplay = $"{dto.PatientName!.Trim()}{(dto.Sex is null ? "" : $" / {dto.Sex}")}{(dto.Dob is null ? "" : $" / {dto.Dob:yyyy-MM-dd}")}";
            }
            else
            {
                return Results.BadRequest("Provide PatientId or PatientName.");
            }

            // ---- resolve doctor display and ids
            string? doctorDisplay = null;
            long? doctorId = dto.DoctorId;
            long? labDoctorId = null;

            if (dto.DoctorId is > 0)
            {
                var d = await db.LabDoctors.AsNoTracking()
                    .Where(x => x.LabDoctorId == dto.DoctorId)
                    .Select(x => new { x.LabDoctorId, x.FullName })
                    .SingleOrDefaultAsync(ct);
                if (d is null) return Results.BadRequest("Unknown DoctorId.");
                doctorDisplay = d.FullName;
                labDoctorId = d.LabDoctorId; // LIS-local link
            }
            else if (!string.IsNullOrWhiteSpace(dto.DoctorName))
            {
                doctorDisplay = dto.DoctorName!.Trim();
            }

            // ---- create request
            var now = DateTime.UtcNow;
            var orderNo = $"ORD-{now:yyyyMMddHHmmss}";

            var req = new myLabRequest
            {
                OrderNo = orderNo,
                Status = LabRequestStatus.Requested,

                // IMPORTANT: set BOTH *Id and Lab*Id so history/profile work
                PatientId = patientId,     // cross-module (if you use it)
                LabPatientId = labPatientId,  // LIS link (used by history/profile)

                DoctorId = doctorId,
                LabDoctorId = labDoctorId,

                PatientDisplay = patientDisplay,
                DoctorDisplay = doctorDisplay,

                CreatedAt = now,
                CreatedBy = "ui",
            };
            db.LabRequests.Add(req);
            await db.SaveChangesAsync(ct);   // get LabRequestId

            // items
            foreach (var t in tests)
            {
                db.LabRequestItems.Add(new myLabRequestItem
                {
                    LabRequestId = req.LabRequestId,
                    LabTestId = t.LabTestId,
                    LabTestCode = t.Code,
                    LabTestName = t.Name,
                    LabTestUnit = t.Unit,
                    LabTestPrice = t.Price,
                    CreatedAt = now,
                    CreatedBy = "ui"
                });
            }

            // optional: snapshot basic pre-analytics
            if (dto.Pre is not null)
            {
                db.LabPreanalyticals.Add(new myLabPreanalytical
                {
                    LabRequestId = req.LabRequestId,
                    IsDiabetic = dto.Pre.IsDiabetic,
                    TookAntibioticLast3Days = dto.Pre.TookAntibioticLast3Days,
                    FastingHours = dto.Pre.FastingHours,
                    ThyroidStatus = string.IsNullOrWhiteSpace(dto.Pre.ThyroidStatus) ? "None" : dto.Pre.ThyroidStatus,
                    Notes = dto.Pre.Notes,
                    CreatedAt = now,
                    CreatedBy = "ui"
                });
            }

            // sample (collect now?)
            string? acc = null;
            if (dto.CollectNow)
            {
                acc = $"ACC-{now:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
                db.LabSamples.Add(new myLabSample
                {
                    LabRequestId = req.LabRequestId,
                    AccessionNumber = acc,
                    Status = LabSampleStatus.Collected,
                    CreatedAt = now,
                    CreatedBy = "ui"
                });
            }

            await db.SaveChangesAsync(ct);
            return Results.Ok(new { req.LabRequestId, orderNo, accession = acc });
        });





        return app;
    }
}
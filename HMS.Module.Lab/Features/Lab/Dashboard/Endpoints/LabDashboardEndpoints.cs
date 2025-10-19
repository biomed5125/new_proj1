using HMS.Module.Lab.Features.Lab.Models.Enums; // for LabResultStatus
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class LabDashboardEndpoints
{
    public static IEndpointRouteBuilder MapLabDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/dashboard").WithTags("Lab Dashboard");

        // ---- A) Latest Orders (patient-first) ----
        g.MapGet("/orders", async ([FromServices] LabDbContext db, CancellationToken ct) =>
        {
            var raw = await db.LabRequests
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .Take(20)
                .Select(r => new
                {
                    r.LabRequestId,
                    r.OrderNo,
                    r.PatientDisplay,
                    r.DoctorDisplay,           // ✅ NEW
                    r.Source,                  // ✅ NEW

                    Tests = r.Items
                        .Where(i => !i.IsDeleted)
                        .OrderBy(i => i.LabRequestItemId)
                        .Select(i => i.LabTestCode ?? (i.LabTest != null ? i.LabTest.Code : null))
                        .Where(c => c != null)
                        .ToArray(),

                    HasAny = db.LabResults.Any(x => x.LabRequestId == r.LabRequestId && !x.IsDeleted),
                    AllFinal = db.LabResults
                        .Where(x => x.LabRequestId == r.LabRequestId && !x.IsDeleted)
                        .All(x => x.Status == LabResultStatus.Final),

                    r.UpdatedAt,
                    r.CreatedAt
                })
                .ToListAsync(ct);

            var rows = raw.Select(x => new
            {
                x.LabRequestId,
                orderNo = x.OrderNo,
                patient = x.PatientDisplay,
                doctor = x.DoctorDisplay,   // ✅ expose
                source = x.Source,          // ✅ expose
                tests = x.Tests,
                status = !x.HasAny ? "Requested" : (x.AllFinal ? "Final" : "Partial"),
                atUtc = (x.UpdatedAt ?? x.CreatedAt).ToUniversalTime()
            });

            return Results.Ok(rows);
        })
        .WithName("Lab_Dashboard_Orders_v1");



        // ---- B) Recent Results (patient-first) ----
        g.MapGet("/results", async ([FromServices] LabDbContext db, CancellationToken ct) =>
        {
            var rows = await
            (
                from r in db.LabResults.AsNoTracking()
                join req in db.LabRequests.AsNoTracking()
                    on r.LabRequestId equals req.LabRequestId
                join t in db.LabTests.AsNoTracking()
                    on r.LabTestId equals t.LabTestId
                where !r.IsDeleted && !req.IsDeleted
                orderby (r.UpdatedAt ?? r.CreatedAt) descending
                select new
                {
                    accessionNo = r.AccessionNumber,
                    code = t.Code,
                    name = t.Name,
                    value = r.Value,
                    unit = r.Unit ?? t.Unit,
                    flag = r.Flag,
                    status = r.Status.ToString(),
                    createdAtUtc = r.CreatedAt.ToUniversalTime(),
                    updatedAtUtc = (r.UpdatedAt ?? r.CreatedAt).ToUniversalTime(),

                    patient = req.PatientDisplay,   // ✅ NEW
                    doctor = req.DoctorDisplay,    // ✅ NEW
                    source = req.Source            // ✅ NEW
                }
            )
            .Take(200)
            .ToListAsync(ct);

            return Results.Ok(rows);
        })
        .WithName("Lab_Dashboard_Results_v1");

        app.MapGet("/api/v1/lab/dashboard/cards", async (LabDbContext db, CancellationToken ct) =>
        {
            var totalRequests = await db.LabRequests.CountAsync(ct);
            var totalOrders = totalRequests;
            var pendingSamples = await db.LabSamples.CountAsync(s => s.Status != LabSampleStatus.Received && !s.IsDeleted, ct);
            var totalResults = await db.LabResults.CountAsync(ct);
            var finalResults = await db.LabResults.CountAsync(r => r.Status == LabResultStatus.Final, ct);

            return Results.Ok(new { totalRequests, totalOrders, pendingSamples, totalResults, finalResults });
        })
        .WithTags("Dashboard");

        //----------------- Pending samples API (used by /lab/samples page)
        app.MapGet("/api/v1/lab/dashboard/samples/pending", async (LabDbContext db, CancellationToken ct) =>
        {
            var rows = await db.LabSamples.AsNoTracking()
                .Where(s => !s.IsDeleted && s.Status != LabSampleStatus.Received)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new {
                    labSampleId = s.LabSampleId,
                    labRequestId = s.LabRequestId,
                    accessionNumber = s.AccessionNumber,
                    status = s.Status.ToString(),
                    createdAtUtc = s.CreatedAt
                })
                .Take(200)
                .ToListAsync(ct);

            return Results.Ok(rows);
        });


        return app;
    }
}

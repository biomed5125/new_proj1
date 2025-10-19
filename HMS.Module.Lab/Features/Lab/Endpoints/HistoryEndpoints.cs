// HMS.Module.Lab/Features/Lab/Endpoints/HistoryEndpoints.cs
using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;
public static class LabHistoryEndpoints
{
    public static IEndpointRouteBuilder MapLabHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        // inside MapLabHistoryEndpoints
        var g = app.MapGroup("/api/v1/lab/history").WithTags("Laboratory");

        // FIX: drop the extra "history" segment
        g.MapGet("/patient/{id:long}", async (
            long id, int? days, DateTime? from, DateTime? to, LabDbContext db, CancellationToken ct) =>
        {
            DateTime toUtc = (to?.Kind == DateTimeKind.Utc ? to.Value : (to ?? DateTime.UtcNow)).Date.AddDays(1).AddTicks(-1);
            DateTime fromUtc = (from?.Kind == DateTimeKind.Utc ? from.Value : (from ?? toUtc.AddDays(-(days ?? 30)))).Date;

            var q =
                from r in db.LabRequests.AsNoTracking()
                where !r.IsDeleted && (r.PatientId == id || r.LabPatientId == id)
                join s in db.LabSamples.AsNoTracking() on r.LabRequestId equals s.LabRequestId into sg
                from s in sg.DefaultIfEmpty()
                join i in db.LabRequestItems.AsNoTracking() on r.LabRequestId equals i.LabRequestId
                join t in db.LabTests.AsNoTracking() on i.LabTestId equals t.LabTestId
                let latest = db.LabResults.AsNoTracking()
                                .Where(x => x.LabRequestItemId == i.LabRequestItemId)
                                .OrderByDescending(x => x.CreatedAt)
                                .FirstOrDefault()
                let atUtc = (DateTime?)latest.CreatedAt ?? r.UpdatedAt ?? r.CreatedAt
                where atUtc >= fromUtc && atUtc <= toUtc
                orderby atUtc descending, r.OrderNo
                select new
                {
                    atUtc,
                    orderNo = r.OrderNo,
                    accession = s.AccessionNumber,
                    test = t.Code,                                   // <— note: “test” (singular)
                    value = latest != null ? latest.Value : null,
                    unit = (latest != null ? latest.Unit : null) ?? t.Unit,
                    status = latest != null ? latest.Status.ToString() : r.Status.ToString(),
                    source = r.Source ?? "-"
                };

            return Results.Ok(await q.Take(500).ToListAsync(ct));
        });

        // -------------------------
        // DOCTOR HISTORY (unchanged)
        // GET /api/v1/lab/history/doctor/{id}?days=30
        // -------------------------
        g.MapGet("/doctor/{id:long}", async (
            long id,
            [FromQuery] int days,
            LabDbContext db, CancellationToken ct) =>
        {
            var d = days > 0 ? days : 30;
            var since = DateTime.UtcNow.AddDays(-d);

            var q =
                from r in db.LabRequests.AsNoTracking()
                where !r.IsDeleted
                   && (r.DoctorId == id || r.LabDoctorId == id)
                   && r.CreatedAt >= since
                let reqId = r.LabRequestId
                select new
                {
                    atUtc = r.CreatedAt,
                    patient = r.PatientDisplay,
                    tests = string.Join(",",
                        db.LabRequestItems.Where(i => i.LabRequestId == reqId && !i.IsDeleted)
                          .OrderBy(i => i.LabTestCode)
                          .Select(i => i.LabTestCode ?? (i.LabTest != null ? i.LabTest.Code : null))),
                    status = (db.LabResults.Any(x => x.LabRequestId == reqId && x.Status == LabResultStatus.Final))
                                ? "Final"
                                : (db.LabResults.Any(x => x.LabRequestId == reqId)) ? "Partial" : "Requested",
                    source = r.Source ?? "-",
                    orderNo = r.OrderNo
                };

            var rows = await q.OrderByDescending(x => x.atUtc).Take(500).ToListAsync(ct);
            return Results.Ok(rows);
        })
        .WithName("Lab_History_Doctor_v1");

        return g;
    }
}

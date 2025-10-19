//using HMS.Module.Lab.Features.Lab.Models.Enums;
//using HMS.Module.Lab.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;
//using HMS.Api.Features.Lab.Models.Dtos;

//namespace HMS.Api.Features.Dashboard.Lab;

//public static class LabDashboardEndpoints
//{
//    public static RouteGroupBuilder MapLabDashboardEndpoints(this RouteGroupBuilder g)
//    {
//        // ---- Today’s orders (join items -> tests via LabTestId) ----------------
//        g.MapGet("/orders/today", async (LabDbContext db) =>
//        {
//            var start = DateTime.UtcNow.Date;

//            // 1) base orders (simple, fast)
//            var baseRows = await db.LabRequests
//                .AsNoTracking()
//                .Where(r => !r.IsDeleted && r.CreatedAt >= start)
//                .OrderByDescending(r => r.CreatedAt)
//                .Select(r => new
//                {
//                    RequestId = r.LabRequestId,
//                    r.OrderNo,
//                    r.PatientId,
//                    Status = r.Status.ToString(),
//                    CreatedAtUtc = r.CreatedAt
//                })
//                .Take(200)
//                .ToListAsync();

//            var reqIds = baseRows.Select(r => r.RequestId).ToArray();
//            if (reqIds.Length == 0) return Results.Ok(Array.Empty<OrderRow>());

//            // 2) fetch test codes for those requests using an explicit join
//            var codesFlat = await (
//                from i in db.LabRequestItems.AsNoTracking()
//                join t in db.LabTests.AsNoTracking()
//                    on i.LabTestId equals t.LabTestId
//                where !i.IsDeleted && !t.IsDeleted && reqIds.Contains(i.LabRequestId)
//                select new { i.LabRequestId, t.Code }
//            ).ToListAsync();

//            var codesByReq = codesFlat
//                .GroupBy(x => x.LabRequestId)
//                .ToDictionary(gp => gp.Key, gp => gp.Select(x => x.Code).ToList());

//            // 3) materialize DTOs
//            var result = baseRows.Select(r =>
//                new OrderRow(
//                    r.RequestId,
//                    r.OrderNo,
//                    r.PatientId,
//                    Tests: string.Join(",", (codesByReq.TryGetValue(r.RequestId, out var list) ? list : new List<string>()).Take(5)),
//                    Status: r.Status,
//                    CreatedAtUtc: r.CreatedAtUtc
//                )
//            ).ToList();

//            return Results.Ok(result);
//        });

//        // ---- Pending samples (unchanged) ---------------------------------------
//        g.MapGet("/samples/pending", async (LabDbContext db) =>
//        {
//            var rows = await db.LabSamples
//                .AsNoTracking()
//                .Where(s => !s.IsDeleted && s.Status == LabSampleStatus.Collected)
//                .OrderByDescending(s => s.CreatedAt)
//                .Select(s => new SampleRow(
//                    s.LabSampleId,
//                    s.AccessionNumber,
//                    s.LabRequestId,
//                    s.Status.ToString(),
//                    s.CreatedAt))
//                .Take(200)
//                .ToListAsync();

//            return Results.Ok(rows);
//        });

//        // ---- Today’s results (left join to tests by LabTestId) -----------------
//        // Latest results from the last 24 hours (or tweak the window)
//        g.MapGet("/results/today", async (LabDbContext db) =>
//        {
//            var start = DateTime.UtcNow.AddDays(-1);

//            var rows = await
//                (from r in db.LabResults.AsNoTracking()
//                 where !r.IsDeleted && r.CreatedAt >= start
//                 join ri in db.LabRequestItems.AsNoTracking()
//                     on r.LabRequestItemId equals ri.LabRequestItemId
//                 join t in db.LabTests.AsNoTracking()
//                     on ri.LabTestId equals t.LabTestId into jt
//                 from t in jt.DefaultIfEmpty()   // left join
//                 orderby r.CreatedAt descending
//                 select new ResultRow(
//                     /* 1 */ r.LabResultId,
//                     /* 2 */ r.LabRequestId,
//                     /* 3 */ t != null ? t.Code : null,
//                     /* 4 */ r.Value,
//                     /* 5 */ r.Unit,
//                     /* 6 */ r.Status.ToString(),
//                     /* 7 */ r.CreatedAt
//                 ))
//                .Take(200)
//                .ToListAsync();

//            return Results.Ok(rows);
//        })
//               .WithName("LabDashboard_Today")
//        .WithTags("LabDashboard");

//        return g;
//    }
//}

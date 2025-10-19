using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers; // CacheControlHeaderValue

namespace HMS.Module.Lab.Features.Lab.Endpoints.Reports
{
    public static class ReportEndpoints
    {
        public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/api/v1/lab/results").WithTags("Reports");

            // HEAD
            g.MapGet("/report-head", async (
                [FromQuery] string acc,
                LabDbContext db,
                HttpContext http,                 // <— add HttpContext
                CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(acc))
                    return Results.BadRequest("Missing accession number.");

                // hard no-cache headers
                var h = http.Response.GetTypedHeaders();
                h.CacheControl = new CacheControlHeaderValue { NoStore = true, NoCache = true, MustRevalidate = true };
                http.Response.Headers[HeaderNames.Pragma] = "no-cache";
                http.Response.Headers[HeaderNames.Expires] = "0";

                var head = await (
                    from s in db.LabSamples.AsNoTracking()
                    join r in db.LabRequests.AsNoTracking() on s.LabRequestId equals r.LabRequestId
                    where s.AccessionNumber == acc && !s.IsDeleted
                    select new
                    {
                        accession = s.AccessionNumber,
                        orderNo = r.OrderNo,
                        status = s.Status.ToString(),
                        createdAtUtc = r.CreatedAt,
                        patientDisplay = r.PatientDisplay,
                        doctorDisplay = r.DoctorDisplay
                    }
                ).SingleOrDefaultAsync(ct);

                return head is null ? Results.NotFound() : Results.Ok(head);
            })
            .WithName("Lab_Report_Head_v1");

            // ROWS
            g.MapGet("/report-rows", async (
                [FromQuery] string acc,
                LabDbContext db,
                HttpContext http,                 // <— add HttpContext
                CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(acc))
                    return Results.BadRequest("Missing accession number.");

                // hard no-cache headers
                var h = http.Response.GetTypedHeaders();
                h.CacheControl = new CacheControlHeaderValue { NoStore = true, NoCache = true, MustRevalidate = true };
                http.Response.Headers[HeaderNames.Pragma] = "no-cache";
                http.Response.Headers[HeaderNames.Expires] = "0";

                var rows = await db.LabResults
                    .AsNoTracking()
                    .Include(r => r.LabTest)
                    .Where(r => r.AccessionNumber == acc && !r.IsDeleted)
                    .OrderBy(r => r.LabTest.Code)
                    .Select(r => new
                    {
                        code = r.LabTest.Code,
                        name = r.LabTest.Name,
                        value = r.Value,
                        unit = r.LabTest.Unit,
                        refLow = r.RefLow,
                        refHigh = r.RefHigh,
                        flag = r.Flag
                    })
                    .ToListAsync(ct);

                return Results.Ok(rows);
            })
            .WithName("Lab_Report_Rows_v1");

            app.MapGet("/api/v1/lab/reports/cbc/{orderNo}", (string orderNo) =>
            {
                // 👇 demo data now (replace later by querying LabResults/Patient/Sample)
                var now = DateTime.UtcNow;
                var dto = new CbcReportDto
                {
                    OrderNo = orderNo,
                    Accession = "ACC-000123",
                    PatientName = "Demo Patient",
                    PatientId = "P0001",
                    CollectedAt = now.AddHours(-2),
                    ReportedAt = now,

                    WBC = 9.56m,
                    RBC = 4.68m,
                    HGB = 10.4m,
                    HCT = 33.4m,
                    MCV = 71.5m,
                    MCH = 22.3m,
                    MCHC = 31.3m,
                    RDW_CV = 16.6m,
                    PLT = 336m,
                    MPV = 11.1m,

                    NEUT_Pct = 59.1m,
                    LYMPH_Pct = 32.9m,
                    MONO_Pct = 5.5m,
                    EO_Pct = 2.2m,
                    BASO_Pct = 0.3m,

                    MorphologyNote = "RBC MORPHOLOGY: Hypochromia / Anisocytosis / Microcytosis."
                };

                // simple refs (adjust to your policy)
                dto.Ref["HGB"] = (11.5m, 15.5m);
                dto.Ref["RBC"] = (3.8m, 5.2m);
                dto.Ref["HCT"] = (35m, 47m);
                dto.Ref["MCV"] = (78m, 98m);
                dto.Ref["MCH"] = (26m, 34m);
                dto.Ref["MCHC"] = (30m, 35m);
                dto.Ref["RDW_CV"] = (null, 14.5m);
                dto.Ref["WBC"] = (4.0m, 10.0m);
                dto.Ref["PLT"] = (150m, 450m);
                dto.Ref["MPV"] = (7.5m, 12.5m);

                // demo histograms (32 bins): shape roughly like analyzer prints
                float[] bump(float c, float w, float h)
                {
                    var n = 32; var arr = new float[n];
                    for (int i = 0; i < n; i++)
                    {
                        var x = i / (float)(n - 1);
                        var y = (float)(h * Math.Exp(-0.5 * Math.Pow((x - c) / w, 2)));
                        arr[i] = y;
                    }
                    return arr;
                }

                dto.RbcHist = bump(0.45f, 0.09f, 1.0f);                 // single peak
                dto.PltHist = bump(0.25f, 0.08f, 0.9f);                  // left-skewed
                var w1 = bump(0.25f, 0.07f, 0.6f);                       // lymphs
                var w2 = bump(0.55f, 0.08f, 0.9f);                       // neutrophils
                dto.WbcHist = w1.Zip(w2, (a, b) => (float)(a + b * 1.1)).ToArray();

                return Results.Ok(dto);
            });

            return app;
        }
    }

}

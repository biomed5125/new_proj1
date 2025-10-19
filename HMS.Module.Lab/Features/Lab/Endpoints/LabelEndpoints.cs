using HMS.Module.Lab.Features.Lab.Models.Enums;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Features.Lab.Endpoints;

public static class LabelEndpoints
{
    public sealed record LabelDto(
        string OrderNo,
        string Accession,
        string? Patient,
        string? Doctor,
        DateTime CreatedAtUtc,
        string[] Tests,
        string SampleStatus
    );

    public static IEndpointRouteBuilder MapLabelEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/labels").WithTags("Labels");

        // GET /api/v1/lab/labels/by-accession/ACC-...
        g.MapGet("/by-accession/{accession}", async (
            string accession,
            LabDbContext db,
            CancellationToken ct) =>
        {
            var data = await (from s in db.LabSamples.AsNoTracking()
                              join r in db.LabRequests.AsNoTracking() on s.LabRequestId equals r.LabRequestId
                              where s.AccessionNumber == accession && !s.IsDeleted
                              select new
                              {
                                  r.OrderNo,
                                  s.AccessionNumber,
                                  r.PatientDisplay,
                                  r.DoctorDisplay,
                                  r.CreatedAt,
                                  r.LabRequestId,
                                  Status = s.Status
                              })
                              .SingleOrDefaultAsync(ct);

            if (data is null) return Results.NotFound(new { message = "Accession not found." });

            var tests = await (from i in db.LabRequestItems.AsNoTracking()
                               join t in db.LabTests.AsNoTracking() on i.LabTestId equals t.LabTestId
                               where i.LabRequestId == data.LabRequestId
                               orderby t.Code
                               select $"{t.Code}")
                               .ToArrayAsync(ct);

            return Results.Ok(new LabelDto(
                data.OrderNo,
                data.AccessionNumber,
                data.PatientDisplay,
                data.DoctorDisplay,
                DateTime.SpecifyKind(data.CreatedAt, DateTimeKind.Utc),
                tests,
                data.Status.ToString()
            ));
        });

        return app;
    }
}

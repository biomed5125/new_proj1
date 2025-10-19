using HMS.Module.Lab.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HMS.Module.Lab.Features.Lab.Endpoints;
public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapLabAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/lab/admin").WithTags("Laboratory");

        //g.MapPost("/seed-local", (IServiceProvider sp, CancellationToken ct)
        //    => LabDbSeeder.SeedAsync(sp, ct)).WithDescription("Idempotent local seed");

        //g.MapPost("/seed-cross", (IServiceProvider sp, CancellationToken ct)
        //    => LabDbSeeder.SeedLocalAsync(sp, ct)).WithDescription("Requires other modules present");

        // somewhere inside your Lab API service/endpoint
        //var commRepo = sp.GetRequiredService<ICommRepository>();

        //var outbox = new
        //{
        //    Mrn = "P00000001",
        //    PatientName = "ALI HASSAN",
        //    OrderNo = "LR20250825-000123",
        //    AccessionNumber = "S2508251201023",        // your barcode/AccessionNumber
        //    TestCodes = new[] { "GLU", "K", "NA" },    // instrument codes from map
        //    Dob = new DateTime(1985, 05, 14),
        //    Sex = "M"
        //};
        //await commRepo.EnqueueOutboundAsync(deviceId: 1, payload: System.Text.Json.JsonSerializer.Serialize(outbox), ct);

        return app;
    }
}

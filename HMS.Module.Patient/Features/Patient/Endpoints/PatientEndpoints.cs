using HMS.Module.Patient.Features.Patient.Models.Dtos;
using HMS.Module.Patient.Features.Patient.Queries;
using HMS.Module.Patient.Features.Patient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HMS.Module.Patient.Features.Patient.Endpoints;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/patients").WithTags("Patients");

        g.MapGet("/", async (IPatientService svc, CancellationToken ct)
            => Results.Ok(await svc.ListAsync(new PatientQuery(), ct)));

        g.MapGet("/{id:long}", async (long id, IPatientService svc, CancellationToken ct)
            => (await svc.GetAsync(id, ct)) is { } p ? Results.Ok(p) : Results.NotFound());

        g.MapPost("/", async (CreatePatientDto dto, IPatientService svc, CancellationToken ct) =>
        {
            var res = await svc.CreateAsync(dto, "api", ct);
            return res.Succeeded
                ? Results.Created($"/api/v1/patients/{res.Value.PatientId}", res.Value)
                : Results.BadRequest(new { error = res.Errors });
        });

        g.MapPut("/{id:long}", async (long id, UpdatePatientDto dto, IPatientService svc, CancellationToken ct) =>
        {
            var res = await svc.UpdateAsync(id, dto, "api", ct);
            return res.Succeeded
                ? Results.Ok(res.Value)
                : Results.NotFound(new { error = res.Errors });
        });

        g.MapDelete("/{id:long}", async (long id, IPatientService svc, CancellationToken ct) =>
        {
            var res = await svc.DeleteAsync(id, "api", ct);
            return res.Succeeded ? Results.NoContent() : Results.NotFound(new { error = res.Errors });
        });

        return app;
    }
}

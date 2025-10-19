// Features/Doctor/Endpoints/DoctorEndpoints.cs
using HMS.Module.Doctor.Features.Doctor.Models.Dtos;
using HMS.Module.Doctor.Features.Doctor.Queries;
using HMS.Module.Doctor.Features.Doctor.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HMS.Module.Doctor.Features.Doctor.Endpoints;

public static class DoctorEndpoints
{
    public static IEndpointRouteBuilder MapDoctorEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/doctors").WithTags("Doctors");

        g.MapGet("/", async (string? search, bool? isActive, IDoctorService svc, CancellationToken ct) =>
        {
            var q = new DoctorQuery { Search = search, IsActive = isActive };
            return Results.Ok(await svc.ListAsync(q, ct));
        });

        g.MapGet("/{id:long}", async (long id, IDoctorService svc, CancellationToken ct)
            => (await svc.GetAsync(id, ct)) is { } d ? Results.Ok(d) : Results.NotFound());

        g.MapPost("/", async (CreateDoctorDto dto, IDoctorService svc, CancellationToken ct) =>
        {
            var res = await svc.CreateAsync(dto, "api", ct);
            return res.Succeeded
                ? Results.Created($"/api/v1/doctors/{res.Value!.DoctorId}", res.Value)
                : Results.BadRequest(new { errors = res.Errors });
        });

        g.MapPut("/{id:long}", async (long id, UpdateDoctorDto dto, IDoctorService svc, CancellationToken ct) =>
        {
            var res = await svc.UpdateAsync(id, dto, "api", ct);
            return res.Succeeded
                ? Results.Ok(res.Value)
                : Results.NotFound(new { errors = res.Errors });
        });

        g.MapDelete("/{id:long}", async (long id, IDoctorService svc, CancellationToken ct)
            => (await svc.DeleteAsync(id, "api", ct)) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}

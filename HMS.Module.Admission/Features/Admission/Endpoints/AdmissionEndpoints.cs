using HMS.Module.Admission.Features.Admission.Models.Dtos;
using HMS.Module.Admission.Features.Admission.Queries;
using HMS.Module.Admission.Features.Admission.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HMS.Module.Admission.Features.Admission.Endpoints;

public static class AdmissionEndpoints
{
    public static IEndpointRouteBuilder MapAdmissionEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/admissions").WithTags("Admissions");

        // LIST
        g.MapGet("/", async (IAdmissionService svc, CancellationToken ct) =>
        {
            var list = await svc.ListAsync(new AdmissionQuery(), ct); // pass query + ct
            return Results.Ok(list);
        });

        // GET BY ID
        g.MapGet("/{id:long}", async (long id, IAdmissionService svc, CancellationToken ct) =>
        {
            var dto = await svc.GetAsync(id, ct);
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        });

        // CREATE  (Result, non-generic)
        g.MapPost("/", async (CreateAdmissionDto dto, IAdmissionService svc, CancellationToken ct) =>
        {
            var res = await svc.CreateAsync(dto, "api", ct);
            return res.Succeeded ? Results.Ok() : Results.BadRequest(new { error = res.Errors });
        });

        // UPDATE  (Result, non-generic)
        g.MapPut("/{id:long}", async (long id, UpdateAdmissionDto dto, IAdmissionService svc, CancellationToken ct) =>
        {
            var res = await svc.UpdateAsync(id, dto, "api", ct);
            return res.Succeeded ? Results.Ok() : Results.BadRequest(new { error = res.Errors });
        });

        // DISCHARGE  (Result, non-generic)
        g.MapPost("/{id:long}/discharge", async (long id, DischargeAdmissionDto dto, IAdmissionService svc, CancellationToken ct) =>
        {
            var res = await svc.DischargeAsync(id, dto, "api", ct);
            return res.Succeeded ? Results.Ok() : Results.BadRequest(new { error = res.Errors });
        });

        // DELETE (soft)  (Result, non-generic)
        g.MapDelete("/{id:long}", async (long id, IAdmissionService svc, CancellationToken ct) =>
        {
            var res = await svc.DeleteAsync(id, "api", ct);
            return res.Succeeded ? Results.NoContent() : Results.BadRequest(new { error = res.Errors });
        });

        return app;
    }
}

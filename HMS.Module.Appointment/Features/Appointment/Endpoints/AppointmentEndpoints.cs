using HMS.Module.Appointment.Features.Appointment.Models.Dtos;
using HMS.Module.Appointment.Features.Appointment.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HMS.Module.Appointment.Features.Appointment.Endpoints;

public static class AppointmentEndpoints
{
    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/appointments").WithTags("Appointments");

        g.MapGet("/", async (
            DateTime? fromUtc,
            DateTime? toUtc,
            long? patientId,
            long? doctorId,
            IAppointmentService svc,
            CancellationToken ct) =>
        {
            var list = await svc.ListAsync(fromUtc, toUtc, patientId, doctorId, ct);
            return Results.Ok(list);
        });

        g.MapGet("/{id:long}", async (long id, IAppointmentService svc, CancellationToken ct)
            => (await svc.GetAsync(id, ct)) is { } a ? Results.Ok(a) : Results.NotFound());

        g.MapPost("/", async (CreateAppointmentDto dto, IAppointmentService svc, CancellationToken ct) =>
        {
            var created = await svc.CreateAsync(dto, "api", ct);
            return Results.Created($"/api/v1/appointments/{created.AppointmentId}", created);
        });

        g.MapPut("/{id:long}", async (long id, UpdateAppointmentDto dto, IAppointmentService svc, CancellationToken ct) =>
        {
            dto.AppointmentId = id;
            var updated = await svc.UpdateAsync(dto, "api", ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        g.MapDelete("/{id:long}", async (long id, IAppointmentService svc, CancellationToken ct)
            => await svc.CancelAsync(id, "api", ct) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}

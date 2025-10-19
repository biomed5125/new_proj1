using HMS.Module.Appointment.Features.Appointment.Models.Entities;

namespace HMS.Module.Appointment.Features.Appointment.Repositories;

public interface IAppointmentWriteRepo
{
    Task AddAsync(myAppointment a, CancellationToken ct);
    Task<myAppointment?> GetAsync(long id, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}

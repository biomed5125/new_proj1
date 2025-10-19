using HMS.Module.Appointment.Features.Appointment.Models.Dtos;
using HMS.SharedKernel.Results;

namespace HMS.Module.Appointment.Features.Appointment.Services;
public interface IAppointmentService
{
    Task<AppointmentDto?> GetAsync(long id, CancellationToken ct);
    Task<List<AppointmentDto>> ListAsync(DateTime? fromUtc, DateTime? toUtc, long? patientId, long? doctorId, CancellationToken ct);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, string? user, CancellationToken ct);
    Task<AppointmentDto?> UpdateAsync(UpdateAppointmentDto dto, string? user, CancellationToken ct);
    Task<bool> CancelAsync(long id, string? user, CancellationToken ct);


}

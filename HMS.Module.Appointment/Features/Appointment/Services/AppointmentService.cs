using HMS.Module.Appointment.Features.Appointment.Models.Dtos;
using HMS.Module.Appointment.Features.Appointment.Models.Entities;
using HMS.Module.Appointment.Features.Appointment.Models.Enums;
using HMS.Module.Appointment.Features.Appointment.Repositories;
using HMS.SharedKernel.Ids;
using HMS.SharedKernel.Results;

namespace HMS.Module.Appointment.Features.Appointment.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentReadRepo _read;
    private readonly IAppointmentWriteRepo _write;
    private readonly IBusinessIdGenerator _ids;

    public AppointmentService(IAppointmentReadRepo read, IAppointmentWriteRepo write, IBusinessIdGenerator ids)
    { _read = read; _write = write; _ids = ids; }

    private static string NewApptNo(DateTime whenUtc) => $"AP{whenUtc:yyyyMMddHHmmssfff}";
    private static AppointmentDto ToDto(myAppointment a) => new AppointmentDto
    {
        AppointmentId = a.AppointmentId,
        AppointmentNo = a.AppointmentNo,
        PatientId = a.PatientId,
        DoctorId = a.DoctorId,
        ScheduledAtUtc = a.ScheduledAtUtc,
        DurationMinutes = a.DurationMinutes,
        Status = (AppointmentStatus)(int)a.Status,
        Reason = a.Reason,
        Notes = a.Notes,
        CreatedAt = a.CreatedAt
    };
    public async Task<AppointmentDto?> GetAsync(long id, CancellationToken ct)
        => (await _read.GetAsync(id, ct)) is { } e ? ToDto(e) : null;

    public async Task<List<AppointmentDto>> ListAsync(
        DateTime? fromUtc, DateTime? toUtc, long? patientId, long? doctorId, CancellationToken ct)
        => (await _read.ListAsync(fromUtc, toUtc, patientId, doctorId, ct)).Select(ToDto).ToList();

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, string? user, CancellationToken ct)
    {
        var e = new myAppointment
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            ScheduledAtUtc = dto.ScheduledAtUtc,
            DurationMinutes = dto.DurationMinutes,
            Status = AppointmentStatus.Scheduled,
            Reason = dto.Reason,
            Notes = dto.Notes,
            AppointmentNo = _ids.NewAppointmentNo(dto.ScheduledAtUtc),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user
        };

        await _write.AddAsync(e, ct);
        await _write.SaveAsync(ct);
        return ToDto(e);
    }

    public async Task<AppointmentDto?> UpdateAsync(UpdateAppointmentDto dto, string? user, CancellationToken ct)
    {
        // Load the entity in the WRITE context so changes are tracked
        var e = await _write.GetAsync(dto.AppointmentId, ct);
        if (e is null) return null;

        e.ScheduledAtUtc = dto.ScheduledAtUtc;
        e.DurationMinutes = dto.DurationMinutes;
        e.Reason = dto.Reason;
        e.Notes = dto.Notes;
        e.Status = (AppointmentStatus)dto.Status;
        e.UpdatedAt = DateTime.UtcNow;
        e.UpdatedBy = user;

        await _write.SaveAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> CancelAsync(long id, string? user, CancellationToken ct)
    {
        var e = await _write.GetAsync(id, ct);
        if (e is null) return false;

        if (e.Status == AppointmentStatus.Cancelled) return true; // idempotent

        e.Status = AppointmentStatus.Cancelled;
        e.UpdatedAt = DateTime.UtcNow;
        e.UpdatedBy = user;

        await _write.SaveAsync(ct);
        return true;
    }
}

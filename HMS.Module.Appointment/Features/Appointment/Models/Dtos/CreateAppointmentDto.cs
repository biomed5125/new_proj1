using HMS.Module.Appointment.Features.Appointment.Models.Enums;

namespace HMS.Module.Appointment.Features.Appointment.Models.Dtos;
public sealed class CreateAppointmentDto
{
    public long PatientId { get; set; }
    public long? DoctorId { get; set; }
    public DateTime ScheduledAtUtc { get; set; }     // client sends UTC
    public int DurationMinutes { get; set; } = 30;
    public AppointmentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

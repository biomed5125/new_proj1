using HMS.Module.Appointment.Features.Appointment.Models.Enums;

namespace HMS.Module.Appointment.Features.Appointment.Models.Dtos;
public sealed class UpdateAppointmentDto
{
    public long AppointmentId { get; set; }
    public long PatientId { get; set; }
    public long? DoctorId { get; set; }
    public DateTime ScheduledAtUtc { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

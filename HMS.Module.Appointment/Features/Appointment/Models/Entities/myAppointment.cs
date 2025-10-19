using HMS.Module.Appointment.Features.Appointment.Models.Enums;
using HMS.SharedKernel.Base;

namespace HMS.Module.Appointment.Features.Appointment.Models.Entities;
public class myAppointment : BaseEntity<long>
{
    public long AppointmentId
    {
        get => Id;
        set => Id = value;
    }

    public long PatientId { get; set; }
    public long? DoctorId { get; set; }          // optional until Doctor module is ready

    /// <summary>UTC start time.</summary>
    public DateTime ScheduledAtUtc { get; set; }
    /// <summary>Duration in minutes (15–240 typical)</summary>
    public int DurationMinutes { get; set; } = 30;

    public AppointmentStatus Status { get; set; }

    public string? Reason { get; set; }
    public string? Notes { get; set; }
    // Appointment
    public string? AppointmentNo { get; set; }            // unique per appointment
}

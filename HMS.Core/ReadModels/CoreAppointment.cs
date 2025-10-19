namespace HMS.Core.ReadModels;

public class CoreAppointment
{
    public long AppointmentId { get; set; }
    public string AppointmentNo { get; set; } = "";
    public long PatientId { get; set; }
    public long DoctorId { get; set; }
    public DateTime ScheduledAtUtc { get; set; }
    public int DurationMinutes { get; set; }
    public int Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

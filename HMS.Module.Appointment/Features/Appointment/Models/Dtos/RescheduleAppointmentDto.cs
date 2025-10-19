namespace HMS.Module.Appointment.Features.Appointment.Models.Dtos
{
    public sealed class RescheduleAppointmentDto
    {
        public DateTime ScheduledAtUtc { get; set; }
        public int DurationMinutes { get; set; } = 30;
    }
}

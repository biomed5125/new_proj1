using FluentValidation;
using HMS.Module.Appointment.Features.Appointment.Models.Dtos;

namespace HMS.Module.Appointment.Features.Appointment.Validation;
public sealed class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointmentDto>
{
    public RescheduleAppointmentValidator()
    {
        RuleFor(x => x.ScheduledAtUtc)
            .Must(d => d.Kind == DateTimeKind.Utc)
            .WithMessage("ScheduledAtUtc must be in UTC.");
        RuleFor(x => x.DurationMinutes).InclusiveBetween(5, 8 * 60);
    }
}

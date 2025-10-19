using FluentValidation;
using HMS.Module.Appointment.Features.Appointment.Models.Dtos;

namespace HMS.Module.Appointment.Features.Appointment.Validation;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.DoctorId).GreaterThan(0);
        RuleFor(x => x.ScheduledAtUtc)
            .Must(d => d.Kind == DateTimeKind.Utc)
            .WithMessage("ScheduledAtUtc must be in UTC.");
        RuleFor(x => x.DurationMinutes).InclusiveBetween(5, 24 * 60);
        RuleFor(x => x.Reason).MaximumLength(200);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

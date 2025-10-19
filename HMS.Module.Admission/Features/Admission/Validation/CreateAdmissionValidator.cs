using FluentValidation;
using HMS.Module.Admission.Features.Admission.Models.Dtos;

namespace HMS.Module.Admission.Features.Admission.Validation;

public sealed class CreateAdmissionValidator : AbstractValidator<CreateAdmissionDto>
{
    public CreateAdmissionValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.AdmittedAtUtc).NotEmpty();
        RuleFor(x => x.DiagnosisOnAdmission).MaximumLength(256);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

using FluentValidation;
using HMS.Module.Admission.Features.Admission.Models.Dtos;

namespace HMS.Module.Admission.Features.Admission.Validation;

public sealed class UpdateAdmissionValidator : AbstractValidator<UpdateAdmissionDto>
{
    public UpdateAdmissionValidator()
    {
        RuleFor(x => x.DiagnosisOnAdmission).MaximumLength(256);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

using FluentValidation;
using HMS.Module.Admission.Features.Admission.Models.Dtos;

namespace HMS.Module.Admission.Features.Admission.Validation;

public sealed class DischargeAdmissionValidator : AbstractValidator<DischargeAdmissionDto>
{
    public DischargeAdmissionValidator()
    {
        RuleFor(x => x.DischargedAtUtc).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

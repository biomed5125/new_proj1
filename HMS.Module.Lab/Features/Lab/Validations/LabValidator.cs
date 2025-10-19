using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;

namespace HMS.Module.Lab.Features.Lab.Validation;

public sealed class LabRequestCreateValidator : AbstractValidator<LabRequestDto>
{
    public LabRequestCreateValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.Priority).NotEmpty().MaximumLength(40);
        RuleFor(x => x.LabTestId).NotNull().Must(ts => ts.Count > 0).WithMessage("At least one test is required.");
    }
}
public sealed class LabResultCreateValidator : AbstractValidator<EnterResultDto>
{
    public LabResultCreateValidator()
    {
        RuleFor(x => x.LabRequestItemId).GreaterThan(0);
        RuleFor(x => x.Unit).MaximumLength(20);
        RuleFor(x => x.Flag).MaximumLength(4);
    }
}

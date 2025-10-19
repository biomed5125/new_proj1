using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;

namespace HMS.Module.Lab.Validations;
public sealed class CreateLabRequestValidator : AbstractValidator<CreateLabRequestDto>
{
    public CreateLabRequestValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.Priority).NotEmpty();
        RuleFor(x => x).Must(x => (x.TestIds?.Count ?? 0) + (x.PanelIds?.Count ?? 0) > 0)
            .WithMessage("At least one test or panel is required.");
    }
}

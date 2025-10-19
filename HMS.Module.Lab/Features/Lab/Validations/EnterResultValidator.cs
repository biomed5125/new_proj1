// Validators/ResultValidators.cs

using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;

namespace HMS.Module.Lab.Features.Lab.Validations
{

    public sealed class EnterResultValidator : AbstractValidator<EnterResultDto>
    {
        public EnterResultValidator()
        {
            RuleFor(x => x.LabRequestItemId).GreaterThan(0);
            RuleFor(x => x.Value).NotNull();
        }
    }
    

}

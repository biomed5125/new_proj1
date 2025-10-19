using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;

namespace HMS.Module.Lab.Validations
{
    public sealed class UpsertTestValidator : AbstractValidator<UpsertTestDto>
    {
        public UpsertTestValidator()   // ✅ must match the class name
        {
            RuleFor(x => x.Code).MaximumLength(40);           // code can be blank (auto-gen)
            RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TatMinutes).GreaterThanOrEqualTo(0);
           
        }
    }
}



using FluentValidation;
using HMS.Module.Patient.Features.Patient.Models.Dtos;

namespace HMS.Module.Patient.Features.Patient.Validation
{
    public class CreatePatientValidator : AbstractValidator<CreatePatientDto>
    {
        public CreatePatientValidator()
        {
            RuleFor(x => x.Mrn).NotEmpty().MaximumLength(32);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(64);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(64);
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.Phone).MaximumLength(32);
        }
    }
}

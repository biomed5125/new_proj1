using FluentValidation;
using HMS.Module.Patient.Features.Patient.Models.Dtos;

namespace HMS.Module.Patient.Features.Patient.Validation
{

    public class PatientValidator : AbstractValidator<PatientDto>
    {
        public PatientValidator()
        {
            RuleFor(x => x.PatientId).GreaterThan(0);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Phone).MaximumLength(32);
        }
    }
}
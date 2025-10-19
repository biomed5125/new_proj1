// Features/Doctor/Validation/CreateDoctorValidator.cs
using FluentValidation;
using HMS.Module.Doctor.Features.Doctor.Models.Dtos;

namespace HMS.Module.Doctor.Features.Doctor.Validation;

public sealed class CreateDoctorValidator : AbstractValidator<CreateDoctorDto>
{
    public CreateDoctorValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(30);
    }
}

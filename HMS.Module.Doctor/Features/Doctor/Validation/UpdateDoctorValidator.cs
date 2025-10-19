// Features/Doctor/Validation/UpdateDoctorValidator.cs
using FluentValidation;
using HMS.Module.Doctor.Features.Doctor.Models.Dtos;

namespace HMS.Module.Doctor.Features.Doctor.Validation;

public sealed class UpdateDoctorValidator : AbstractValidator<UpdateDoctorDto>
{
    public UpdateDoctorValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(30);
    }
}

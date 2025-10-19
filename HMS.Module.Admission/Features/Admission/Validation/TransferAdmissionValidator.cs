using FluentValidation;
using HMS.Api.Features.Admission.Models.Dtos;

namespace HMS.Api.Features.Admission.Validation;

public class TransferAdmissionValidator : AbstractValidator<TransferAdmissionDto>
{
    public TransferAdmissionValidator()
    {
        RuleFor(x => x.AdmissionId).GreaterThan(0);
        RuleFor(x => x.NewWardRoomId).GreaterThan(0);
        RuleFor(x => x.TransferAtUtc).Must(d => d.Kind == DateTimeKind.Utc)
            .WithMessage("TransferAtUtc must be UTC.");
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

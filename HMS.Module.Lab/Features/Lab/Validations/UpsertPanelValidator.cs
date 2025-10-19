using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Validations
{
    public sealed class UpsertPanelValidator : AbstractValidator<UpsertPanelDto>
    {
        public UpsertPanelValidator()
        {
            RuleFor(x => x.Code).MaximumLength(40); // optional
            RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
            RuleFor(x => x.TestIds).NotNull().Must(t => t.Count > 0);
        }
    }
}

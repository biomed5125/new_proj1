using FluentValidation;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Validations
{
    public sealed class ApproveResultsValidator : AbstractValidator<ApproveResultsDto>
    {
        public ApproveResultsValidator()
        {
            RuleFor(x => x.LabRequestId).GreaterThan(0);
            RuleFor(x => x.ApprovedByDoctorId).GreaterThan(0);
        }
    }
}

using FluentValidation;
using Shared.Resources.Validation;
using Shared.ResponseModels;
using System;

namespace Shared.Validations;

public class EducatorStaffParentInstitutionValidator : AbstractValidator<EducatorStaffParentInstitutionResponseDTO>
{
    public EducatorStaffParentInstitutionValidator()
    {
        RuleFor(dto => dto.StaffParentInstitutionId).Must(s => s > 0).WithMessage(ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.StartDate).NotEmpty().WithMessage(ValidationResource.NotBeEmpty);
    }
}

using FluentValidation;
using Shared.BaseModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;
using System;
using System.Linq;

namespace Shared.Validations;

public class EducatorProgramBaseValidator<T> : AbstractValidator<T> where T : EducatorProgramBase
{
    protected EducatorProgramBaseValidator()
    {
        RuleFor(dto => dto.DutyType).Must(s => s is not null).WithMessage(ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.ProgramId).Must(s => s > 0).WithMessage(ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.DutyStartDate).NotNull().WithMessage(ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.IsEducationOfficer).NotNull().WithMessage(ValidationResource.NotBeEmpty);
    }
}
public class EducatorProgramValidator : EducatorProgramBaseValidator<EducatorProgramResponseDTO>
{
    public EducatorProgramValidator()
    {
        RuleFor(dto => dto.DutyStartDate).GreaterThan(x => x.LastDutyEndDate).When(x => x.LastDutyEndDate != null && x.Id == null).WithMessage(ValidationResource.DateInconsistent);
        RuleFor(dto => dto.EducationOfficerDutyStartDate).NotNull().When(x => x.IsEducationOfficer == true).WithMessage(ValidationResource.NotBeEmpty);
        //RuleFor(dto => dto.EducationOfficerDocuments).Must(x => x.Any(x => x?.DocumentType == Types.DocumentTypes.EducationOfficerAssignmentLetter) == true).When(x => x.IsEducationOfficer == true).WithMessage(ValidationResource.RequiredDocument);
    }
}

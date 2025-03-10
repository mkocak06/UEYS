using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class StudentRotationBaseValidator<T> : AbstractValidator<T> where T : StudentRotationBase
{
    protected StudentRotationBaseValidator()
    {
        RuleFor(dto => dto.BeginDate).NotEmpty().When(x => x.ProcessDateForExemption == null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProcessDateForExemption).NotEmpty().When(x => x.BeginDate == null).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.EndDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.IsSuccessful).NotNull().WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class StudentRotationValidator : StudentRotationBaseValidator<StudentRotationResponseDTO>
{
    public StudentRotationValidator()
    {
        //RuleFor(dto => dto.EducatorId).Must(s => s > 0).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProgramId).Must(s => s > 0).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class StudentRotationDTOValidator : StudentRotationBaseValidator<StudentRotationDTO>
{
}
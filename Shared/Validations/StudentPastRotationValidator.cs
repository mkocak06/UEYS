using FluentValidation;
using Shared.BaseModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class StudentPastRotationBaseValidator<T> : AbstractValidator<T> where T : StudentRotationBase
{
    protected StudentPastRotationBaseValidator()
    {
        RuleFor(dto => dto.BeginDate).NotEmpty().WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.EndDate).NotEmpty().WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.IsSuccessful).NotNull().WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.ProgramId).Must(s => s > 0).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.EducatorName).NotNull().WithMessage(ValidationResource.Required);
    }
}

public class StudentPastRotationValidator : StudentPastRotationBaseValidator<StudentRotationResponseDTO>
{
    public StudentPastRotationValidator()
    {
    }
}
using FluentValidation;
using Shared.BaseModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class FinishStudentRotationBaseValidator<T> : AbstractValidator<T> where T : StudentRotationBase
{
    protected FinishStudentRotationBaseValidator()
    {
        RuleFor(dto => dto.EndDate).NotEmpty().WithMessage(ValidationResource.Required);
        //RuleFor(dto => dto.IsSuccessful).NotNull().WithMessage(ValidationResource.Required);
    }
}

public class FinishStudentRotationValidator : FinishStudentRotationBaseValidator<StudentRotationResponseDTO>
{
    public FinishStudentRotationValidator()
    {
        RuleFor(dto => dto.EducatorId).Must(s => s > 0).WithMessage(ValidationResource.Required);
    }
}
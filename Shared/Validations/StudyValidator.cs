using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class StudyBaseValidator<T> : AbstractValidator<T> where T : StudyBase
{
    protected StudyBaseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class StudyValidator : StudyBaseValidator<StudyResponseDTO>
{
    public StudyValidator()
    {
    }
}

public class StudyDTOValidator : StudyBaseValidator<StudyDTO>
{
}
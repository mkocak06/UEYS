using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class SpecificEducationBaseValidator<T> : AbstractValidator<T> where T : SpecificEducationBase
{
    protected SpecificEducationBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class SpecificEducationValidator : SpecificEducationBaseValidator<SpecificEducationBase>
{
}

public class SpecificEducationDTOValidator : SpecificEducationBaseValidator<SpecificEducationDTO>
{
}
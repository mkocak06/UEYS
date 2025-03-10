using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class TitleBaseValidator<T> : AbstractValidator<T> where T : TitleBase
{
    protected TitleBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class TitleValidator : TitleBaseValidator<TitleBase>
{
}

public class TitleDTOValidator : TitleBaseValidator<TitleDTO>
{
}
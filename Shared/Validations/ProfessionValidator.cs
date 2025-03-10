using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class ProfessionBaseValidator<T> : AbstractValidator<T> where T : ProfessionBase
{
    protected ProfessionBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class ProfessionValidator : ProfessionBaseValidator<ProfessionResponseDTO>
{
}

public class ProfessionDTOValidator : ProfessionBaseValidator<ProfessionDTO>
{
}
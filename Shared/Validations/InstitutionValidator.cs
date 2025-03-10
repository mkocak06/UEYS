using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class InstitutionBaseValidator<T> : AbstractValidator<T> where T : InstitutionBase
{
    protected InstitutionBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class InstitutionValidator : InstitutionBaseValidator<InstitutionResponseDTO>
{
}

public class InstitutionDTOValidator : InstitutionBaseValidator<InstitutionDTO>
{
}
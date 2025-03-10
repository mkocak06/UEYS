using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class SpecificEducationPlaceBaseValidator<T> : AbstractValidator<T> where T : SpecificEducationPlaceBase
{
    protected SpecificEducationPlaceBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProvinceId).Must(s => s.HasValue && s.Value > 0).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class SpecificEducationPlaceValidator : SpecificEducationPlaceBaseValidator<SpecificEducationPlaceBase>
{
}

public class SpecificEducationPlaceDTOValidator : SpecificEducationPlaceBaseValidator<SpecificEducationPlaceDTO>
{
}
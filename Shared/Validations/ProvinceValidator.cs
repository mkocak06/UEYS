using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class ProvinceBaseValidator<T> : AbstractValidator<T> where T : ProvinceBase
{
    protected ProvinceBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Latitude).Must(p => p >= -85.0 && p < 85.0).When(u => u.Latitude != null).WithMessage(Resources.Validation.ValidationResource.NotAppropriateValue);
        RuleFor(dto => dto.Longitude).Must(p => p >= -180.0 && p <= 180.0).When(u => u.Longitude != null).WithMessage(Resources.Validation.ValidationResource.NotAppropriateValue);
    }
}

public class ProvinceValidator : ProvinceBaseValidator<ProvinceResponseDTO>
{
}

public class ProvinceDTOValidator : ProvinceBaseValidator<ProvinceDTO>
{
}
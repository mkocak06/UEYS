using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class UniversityBaseValidator<T> : AbstractValidator<T> where T : UniversityBase
{
    protected UniversityBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProvinceId).Must(s => s.HasValue && s.Value>0).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Latitude).Must(p => p >= -85.0 && p<85.0).WithMessage(Resources.Validation.ValidationResource.NotAppropriateValue);
        RuleFor(dto => dto.Longitude).Must(p => p >= -180.0 && p <= 180.0).WithMessage(Resources.Validation.ValidationResource.NotAppropriateValue);
        RuleFor(user => user.Email).EmailAddress();
    }
}

public class UniversityValidator : UniversityBaseValidator<UniversityResponseDTO>
{
    public UniversityValidator()
    {
        RuleFor(dto => dto.Province).Must(s => s is not null).WithMessage("Bu alan zorunludur!");
        RuleFor(dto => dto.Institution).Must(s => s is not null).WithMessage("Bu alan zorunludur!");
    }
}

public class UniversityDTOValidator : UniversityBaseValidator<UniversityDTO>
{
}
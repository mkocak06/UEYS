using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using System.IO.Compression;

namespace Shared.Validations;

public class HospitalBaseValidator<T> : AbstractValidator<T> where T : HospitalBase
{
    protected HospitalBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProvinceId).Must(s => s.HasValue && s.Value > 0).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.InstitutionId).Must(s => s.HasValue && s.Value > 0).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(user => user.Email).EmailAddress();
        RuleFor(dto => dto.Latitude).Must(p => p >= -85.0 && p < 85.0).When(u => u.Latitude != null).WithMessage(Resources.Validation.ValidationResource.NotAppropriateValue);
        RuleFor(dto => dto.Longitude).Must(p => p >= -180.0 && p <= 180.0).When(u => u.Longitude != null).WithMessage(Resources.Validation.ValidationResource.NotAppropriateValue);
    }
}


public class HospitalValidator : HospitalBaseValidator<HospitalResponseDTO>
{
    public HospitalValidator()
    {
        RuleFor(dto => dto.Province).Must(s => s is not null).WithMessage("Bu alan zorunludur!");
        When(x => x.Institution is { Name: "YÖK" }, () =>
        {
            RuleFor(x => x.FacultyId)
            .Must(x => x > 0)
            .WithMessage(Resources.Validation.ValidationResource.Required);
        });
    }
}

public class HospitalDTOValidator : HospitalBaseValidator<HospitalDTO>
{
}

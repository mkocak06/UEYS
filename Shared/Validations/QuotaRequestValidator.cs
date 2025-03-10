using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class QuotaRequestBaseValidator<T> : AbstractValidator<T> where T : QuotaRequestBase
{
    protected QuotaRequestBaseValidator()
    {
        RuleFor(dto => dto.Period).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Year).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Type).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ApplicationEndDate).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}
public class QuotaRequestValidator : QuotaRequestBaseValidator<QuotaRequestResponseDTO>
{
}

public class QuotaRequestDTOValidator : QuotaRequestBaseValidator<QuotaRequestDTO>
{
}
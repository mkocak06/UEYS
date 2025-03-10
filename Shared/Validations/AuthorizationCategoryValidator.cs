using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class AuthorizationCategoryBaseValidator<T> : AbstractValidator<T> where T : AuthorizationCategoryBase
{
    protected AuthorizationCategoryBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithName(Resources.Validation.ValidationResource.Name).WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Description).Must(s => !string.IsNullOrWhiteSpace(s)).WithName(Resources.Validation.ValidationResource.Description).WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Duration).Must(s => s >= 0).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.IsActive).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}
public class AuthorizationCategoryValidator : AuthorizationCategoryBaseValidator<AuthorizationCategoryResponseDTO>
{
}

public class AuthorizationCategoryDTOValidator : AuthorizationCategoryBaseValidator<AuthorizationCategoryDTO>
{
}
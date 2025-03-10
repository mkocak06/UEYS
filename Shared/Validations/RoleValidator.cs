using FluentValidation;
using Shared.ResponseModels;
namespace Shared.Validations;

public class RoleValidator : AbstractValidator<RoleResponseDTO>
{
    public RoleValidator()
    {
        RuleFor(dto => dto.RoleName).Must(x=> !string.IsNullOrWhiteSpace(x)).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}
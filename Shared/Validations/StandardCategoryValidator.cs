using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;
namespace Shared.Validations;

public class StandardCategoryValidator : AbstractValidator<StandardCategoryResponseDTO>
{
    public StandardCategoryValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.CategoryCode).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
    }
}
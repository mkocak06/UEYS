using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;
namespace Shared.Validations;

public class StandardValidator : AbstractValidator<StandardResponseDTO>
{
    public StandardValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.StandardCategoryId).Must(x => x > 0 && x != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.CurriculumId).Must(x => x > 0 && x != null).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}
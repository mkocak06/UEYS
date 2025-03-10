using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;

namespace Shared.Validations;

public class DependentProgramValidator : AbstractValidator<DependentProgramResponseDTO>
{
    public DependentProgramValidator()
    {
        RuleFor(dto => dto.Duration).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Unit).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
    }
}

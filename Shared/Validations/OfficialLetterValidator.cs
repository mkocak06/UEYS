using FluentValidation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class OfficialLetterValidator : AbstractValidator<OfficialLetterResponseDTO>
{
    public OfficialLetterValidator()
    {
        RuleFor(dto => dto.Date).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Description).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
    }
}
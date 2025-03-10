using FluentValidation;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class EthicCommitteeValidator : AbstractValidator<EthicCommitteeDecisionResponseDTO>
{
    public EthicCommitteeValidator()
    {
        RuleFor(dto => dto.Number).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Date).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Description).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
    }
}
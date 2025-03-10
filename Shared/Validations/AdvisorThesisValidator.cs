using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;
using Shared.Types;

namespace Shared.Validations;

public class AdvisorThesisBaseValidator<T> : AbstractValidator<T> where T : AdvisorThesisBase
{
    protected AdvisorThesisBaseValidator()
    {
    }
}

public class AdvisorThesisValidator : AdvisorThesisBaseValidator<AdvisorThesisResponseDTO>
{
    public AdvisorThesisValidator()
    {
        RuleFor(dto => dto.AdvisorAssignDate).Must(s => s is not null).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.DeleteReason).Must(s => s is not null).When(x => x.ChangeCoordinator == true).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.DeleteExplanation).Must(s => s is not null).When(x => x.ChangeCoordinator == true && x.DeleteReason == EducatorDeleteReasonType.Other).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.ExpertiseBranchId).Must(s => s is not null && s != 0).When(x => x.Type == AdvisorType.Educator).WithMessage(ValidationResource.Required);
    }
}

public class AdvisorThesisDTOValidator : AdvisorThesisBaseValidator<AdvisorThesisDTO>
{
    public AdvisorThesisDTOValidator()
    {
        //  RuleFor(dto => dto.Subject).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);

    }
}
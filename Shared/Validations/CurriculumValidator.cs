using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class CurriculumBaseValidator<T> : AbstractValidator<T> where T : CurriculumBase
{
    protected CurriculumBaseValidator()
    {
        RuleFor(dto => dto.Version).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.DecisionNo).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Duration).Must(s => s > 0).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.EffectiveDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class CurriculumValidator : CurriculumBaseValidator<CurriculumResponseDTO>
{
   public CurriculumValidator()
    {
        RuleFor(dto => dto.ExpertiseBranchId).Must(s => s > 0).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class CurriculumDTOValidator : CurriculumBaseValidator<CurriculumDTO>
{
}
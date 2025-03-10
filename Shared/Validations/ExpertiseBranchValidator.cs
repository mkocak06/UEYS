using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class ExpertiseBranchBaseValidator<T> : AbstractValidator<T> where T : ExpertiseBranchBase
{
    protected ExpertiseBranchBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.EducatorIndexRateToCapacityIndex).Must(s => s != null && s >= 0 && s <= 100).WithMessage(ValidationResource.NotAppropriateValue);
        RuleFor(dto => dto.PortfolioIndexRateToCapacityIndex).Must(s => s != null && s >= 0 && s <= 100).WithMessage(ValidationResource.NotAppropriateValue);
    }
}

public class ExpertiseBranchValidator : ExpertiseBranchBaseValidator<ExpertiseBranchResponseDTO>
{
}

public class ExpertiseBranchDTOValidator : ExpertiseBranchBaseValidator<ExpertiseBranchDTO>
{
}


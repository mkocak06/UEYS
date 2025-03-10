using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class PortfolioBaseValidator<T> : AbstractValidator<T> where T : PortfolioBase
{
    protected PortfolioBaseValidator()
    {
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Ratio).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ExpertiseBranchId).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class PortfolioValidator : PortfolioBaseValidator<PortfolioResponseDTO>
{
}

public class PortfolioDTOValidator : PortfolioBaseValidator<PortfolioDTO>
{
}
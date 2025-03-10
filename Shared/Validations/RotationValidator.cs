using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class RotationBaseValidator<T> : AbstractValidator<T> where T : RotationBase
{
    protected RotationBaseValidator()
    {
        RuleFor(dto => dto.Duration).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ExpertiseBranchId).Must(s => s != null && s > 0).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class RotationValidator : RotationBaseValidator<RotationResponseDTO>
{
}

public class RotationDTOValidator : RotationBaseValidator<RotationDTO>
{
}
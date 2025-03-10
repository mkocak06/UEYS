using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class PastOpinionFormBaseValidator<T> : AbstractValidator<T> where T : OpinionFormBase
{
    protected PastOpinionFormBaseValidator()
    {
        RuleFor(dto => dto.Result).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.StartDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.EndDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class PastOpinionFormValidator : PastOpinionFormBaseValidator<OpinionFormResponseDTO>
{
    public PastOpinionFormValidator()
    {
    }
}

public class PastOpinionFormDTOValidator : PastOpinionFormBaseValidator<OpinionFormDTO>
{
}
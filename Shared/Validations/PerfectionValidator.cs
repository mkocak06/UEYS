using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class PerfectionBaseValidator<T> : AbstractValidator<T> where T : PerfectionBase
{
    protected PerfectionBaseValidator() // TODO
    {
        //RuleFor(dto => dto.PerfectionGroup).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.PerfectionMethod).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.PerfectionLevel).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.PerfectionName).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.PerfectionSeniorty).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.PerfectionType).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class PerfectionValidator : PerfectionBaseValidator<PerfectionResponseDTO>
{
}

public class PerfectionDTOValidator : PerfectionBaseValidator<PerfectionDTO>
{
}
using FluentValidation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class ProgressReportValidator : AbstractValidator<ProgressReportResponseDTO>
{
    public ProgressReportValidator()
    {
        RuleFor(dto => dto.BeginDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.EndDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Description).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.EducatorId).Must(x => x.HasValue && x > 0).WithMessage("Lütfen bir Tez Danışmanı ekleyin");
    }
}
using FluentValidation;
using Shared.Resources.Validation;
using Shared.ResponseModels;
using System.Linq;

namespace Shared.Validations;

public class SubQuotaRequestValidator : AbstractValidator<SubQuotaRequestResponseDTO>
{
    public SubQuotaRequestValidator()
    {
        RuleFor(dto => dto.SpecialistDoctorCount).Must(s => s != null).WithMessage(ValidationResource.NotMinusNotEmpty);
        RuleFor(dto => dto.StudentCounts).Must(s => s.Count > 0).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.StudentCounts).Must(expertises =>
        {
            return !expertises.Select(x => x.QuotaType).GroupBy(qt => qt).Any(grp => grp.Count() > 1);
        })
          .WithMessage("Aynı kontenjan türü birden fazla eklenemez.");

        RuleForEach(dto => dto.StudentCounts).SetValidator(new StudentCountValidator());
        RuleForEach(dto => dto.SubQuotaRequestPortfolios).SetValidator(new SubQuotaRequestPortfolioValidator());
    }
}

public class StudentCountValidator : AbstractValidator<StudentCountResponseDTO>
{
    public StudentCountValidator()
    {
        RuleFor(dto => dto.RequestedCount).Must(x => x != null && x >= 0).WithMessage(ValidationResource.NotMinusNotEmpty);
        RuleFor(dto => dto.QuotaType).Must(x => x != null).WithMessage(ValidationResource.Required);
    }
}

public class SubQuotaRequestPortfolioValidator : AbstractValidator<SubQuotaRequestPortfolioResponseDTO>
{
    public SubQuotaRequestPortfolioValidator()
    {
        RuleFor(dto => dto.Answer).NotEmpty().WithMessage(ValidationResource.NotMinusNotEmpty);
    }
}
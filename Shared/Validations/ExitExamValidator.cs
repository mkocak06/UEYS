using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;

namespace Shared.Validations;

public class ExitExamValidator : AbstractValidator<ExitExamResponseDTO>
{
    public ExitExamValidator()
    {
        RuleFor(dto => dto.PracticeExamNote).Must(x => x >= 0 && x <= 100).When(x => x.ExamStatus == Types.ExitExamResultType.Concluded).WithMessage("Lütfen 0 ile 100 arasında bir değer giriniz");
        RuleFor(dto => dto.AbilityExamNote).Must(x => x >= 0 && x <= 100).When(x => x.ExamStatus == Types.ExitExamResultType.Concluded).WithMessage("Lütfen 0 ile 100 arasında bir değer giriniz");
        RuleFor(dto => dto.ExamDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.ExamDate).GreaterThan(x=>x.EstimatedEndDate).WithMessage("Sınav tarihi eğitimin tahmini bitiş tarihinden büyük olmalıdır.");
        RuleFor(dto => dto.ExamStatus).NotNull().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.HospitalId).Must(x => x > 0 && x != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Juries).Must(x => x.Count(a => a.JuryType == JuryType.Core) == 5 && x.Count(a => a.JuryType == JuryType.Alternate) == 2).WithMessage("Asil Üye Sayısı 5 kişiden, Yedek Üye sayısı 2 kişiden oluşmalıdır.");
    }
}
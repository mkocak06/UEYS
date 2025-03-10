using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;
namespace Shared.Validations;

public class ThesisDefenceValidator : AbstractValidator<ThesisDefenceResponseDTO>
{
    public ThesisDefenceValidator()
    {
        //RuleFor(dto => dto.Description).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.ExamDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Result).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        //RuleFor(dto => dto.ResultDate).NotEmpty().When(x=>x.Result != DefenceResultType.InProgress).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        //RuleFor(dto => dto.ResultDate).GreaterThan(x=>x.ExamDate).When(x=>x.Result != DefenceResultType.InProgress).WithMessage("Sonuç Tarihi Sınav Tarihinden Büyük Olmalıdır!");
        RuleFor(dto => dto.HospitalId).Must(x => x > 0 && x != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Juries).Must(x => x?.Count(a => a.JuryType == JuryType.Core) >= 3).WithMessage("Asil Üye Sayısı En Az 3 Kişiden Oluşmalıdır!");
        RuleFor(dto => dto.Juries).Must(x => x?.Count(a => a.JuryType == JuryType.Alternate) >= 2).WithMessage("Yedek Üye Sayısı En Az 2 Kişiden Oluşmalıdır.");
    }
}
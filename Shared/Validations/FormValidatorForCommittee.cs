using FluentValidation;
using Shared.Resources.Validation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;
namespace Shared.Validations;

public class FormValidatorForCommittee : AbstractValidator<FormResponseDTO>
{
    public FormValidatorForCommittee()
    {
        RuleFor(x => x.FormStandards)
            .Must(list => list.All(item => item.CommitteeStatement != false || !string.IsNullOrWhiteSpace(item.CommitteeDescription)))
            .WithMessage("Standart Karşılanmıyorsa ilgili açıklama alanını lütfen doldurunuz.");


        RuleFor(dto => dto.FormStandards)
            .Must(expertises =>expertises.All(e => e.CommitteeStatement.HasValue))
            .WithMessage("Lütfen tüm değerlendirme alanlarını doldurunuz");

        RuleFor(c => c.CommitteeOpinionType).Must(x => x != null)
            .WithName("Komisyon Değerlendirmesi")
            .WithMessage(ValidationResource.RequiredField);
    }
}
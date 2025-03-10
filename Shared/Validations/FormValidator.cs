using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;
namespace Shared.Validations;

public class FormValidator : AbstractValidator<FormResponseDTO>
{
    public FormValidator()
    {

        RuleFor(dto => dto.FormStandards).Must(fs =>
        {
            return fs.All(e => e.InstitutionStatement != null);
        })
        .WithMessage("Lütfen tüm alanları doldurunuz");
    }
}
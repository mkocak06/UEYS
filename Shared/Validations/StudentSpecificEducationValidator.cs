using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;
namespace Shared.Validations;

public class StudentSpecificEducationValidator : AbstractValidator<StudentSpecificEducationResponseDTO>
{
    public StudentSpecificEducationValidator()
    {
        RuleFor(dto => dto.SpesificEducationId).Must(x => x > 0 && x != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.SpecificEducationPlaceId).Must(x => x > 0 && x != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Documents).Must(x => x != null && x.Count>0).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}
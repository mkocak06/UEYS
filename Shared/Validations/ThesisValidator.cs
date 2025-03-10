using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class ThesisBaseValidator<T> : AbstractValidator<T> where T : ThesisBase
{
    protected ThesisBaseValidator()
    {

    }
}

public class ThesisValidator : ThesisBaseValidator<ThesisResponseDTO>
{
    public ThesisValidator()
    {
        RuleFor(dto => dto.Subject).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ThesisSubjectType_1).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ThesisSubjectType_2).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.SubjectDetermineDate).Must(s => s != null).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.ThesisTitle).Must(s => !string.IsNullOrWhiteSpace(s)).When(x => !string.IsNullOrEmpty(x.Subject)).WithMessage(Resources.Validation.ValidationResource.Required);
        //RuleFor(dto => dto.ThesisTitleDetermineDate).Must(s => s != null).When(x=>!string.IsNullOrEmpty(x.Subject)).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class ThesisDTOValidator : ThesisBaseValidator<ThesisDTO>
{
    public ThesisDTOValidator()
    {

    }
}
using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System.Security.Cryptography.X509Certificates;

namespace Shared.Validations;

public class ScientificStudyBaseValidator<T> : AbstractValidator<T> where T : ScientificStudyBase
{
    protected ScientificStudyBaseValidator()
    {
        RuleFor(dto => dto.ProcessDate).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Description).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.StudyId).Must(s => s >0).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}


public class ScientificStudyValidator : ScientificStudyBaseValidator<ScientificStudyResponseDTO>
{
    public ScientificStudyValidator()
    {
    }
}

public class ScientificStudyDTOValidator : ScientificStudyBaseValidator<ScientificStudyDTO>
{
}

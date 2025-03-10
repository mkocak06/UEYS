using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class AnnouncementBaseValidator<T> : AbstractValidator<T> where T : AnnouncementBase
{
    protected AnnouncementBaseValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(x => x.Explanation).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class AnnouncementValidator : AnnouncementBaseValidator<AnnouncementResponseDTO>
{
    public AnnouncementValidator()
    {
    }
}

public class AnnouncementDTOValidator : AnnouncementBaseValidator<AnnouncementDTO>
{
}
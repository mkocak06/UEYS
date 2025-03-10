using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class AuthorizationDetailBaseValidator<T> : AbstractValidator<T> where T : AuthorizationDetailBase
{
    protected AuthorizationDetailBaseValidator()
    {
    }
}

public class AuthorizationDetailValidator : AuthorizationDetailBaseValidator<AuthorizationDetailResponseDTO>
{
    public AuthorizationDetailValidator()
    {
        RuleFor(x => x.AuthorizationCategoryId).NotNull().WithMessage(ValidationResource.NotBeEmpty);
        When(x => x.AuthorizationCategory is {Name: "0" or "1" or "9"}, () =>
        {
            RuleFor(x => x.Descriptions)
                .Must(y => y!=null && y.Count>0)
                .WithMessage(ValidationResource.NotBeEmpty);
        });
        When(x => x.AuthorizationDate is not null && x.AuthorizationEndDate is not null, () =>
        {
            RuleFor(x => x.AuthorizationEndDate).GreaterThanOrEqualTo(x => x.AuthorizationDate).WithName("Authorization End Date").WithMessage(ValidationResource.DateInconsistent);
        });

    }
}

public class AuthorizationDetailDTOValidator : AuthorizationDetailBaseValidator<AuthorizationDetailDTO>
{
    public AuthorizationDetailDTOValidator()
    {
    }
}
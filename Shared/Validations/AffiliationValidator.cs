using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class AffiliationBaseValidator<T> : AbstractValidator<T> where T : AffiliationBase
{
    protected AffiliationBaseValidator()
    {
        RuleFor(dto => dto.ProtocolNo).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.ProtocolDate).Must(s => s is not null).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.FacultyId).GreaterThan(0).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.HospitalId).GreaterThan(0).WithMessage(ValidationResource.Required);
        When(x => x.ProtocolDate is not null && x.ProtocolEndDate is not null, () =>
        {
            RuleFor(x => x.ProtocolEndDate).GreaterThan(x => x.ProtocolDate).WithName("Authorization End Date").WithMessage(ValidationResource.DateInconsistent);
        });
    }
}

public class AffiliationValidator : AffiliationBaseValidator<AffiliationResponseDTO>
{
}

public class AffiliationDTOValidator : AffiliationBaseValidator<AffiliationDTO>
{
}
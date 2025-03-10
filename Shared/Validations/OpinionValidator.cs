using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;

namespace Shared.Validations;

public class OpinionBaseValidator<T> : AbstractValidator<T> where T : OpinionFormBase
{
    protected OpinionBaseValidator()
    {
        RuleFor(o => o.ComplianceToWorkingHours_DC).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.DutyResponsibility_DC).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.DutyExecution_DC).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.DutyAccomplishment_DC).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(o => o.ProblemAnalysisAndSolutionAbility).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.OrganizationAndCoordinationAbility).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.CommunicationSkills).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(o => o.RelationsWithOtherStudents).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.RelationsWithEducators).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.RelationsWithOtherEmployees).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.RelationsWithPatients).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(o => o.ProfessionalPracticeAbility).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.Scientificness).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.TeamworkAdaptation).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.ResearchDesire).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.ResearchExecutionAndAccomplish).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.UsingResourcesEfficiently).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.BroadcastingAbility).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(dto => dto.StartDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.EndDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.IsNotified).NotNull().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.EducatorId).GreaterThan(0).NotNull().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.AdditionalExplanation).NotEmpty().When(s => s.IsRepeating == true).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class OpinionValidator : OpinionBaseValidator<OpinionFormResponseDTO>
{
    public OpinionValidator()
    {
        RuleFor(dto => dto.Documents).NotNull().Must(x => x.Any(y => y?.DocumentType == DocumentTypes.OpinionForm) == true).WithMessage("Kanaat dosyası zorunludur!");
        RuleFor(dto => dto.Documents).NotNull().Must(x => x.Any(y => y?.DocumentType == DocumentTypes.Communique) == true).When(x=>x.IsNotified == true).WithMessage("Tebliğ dosyası zorunludur!");
    }
}

public class OpinionDTOValidator : OpinionBaseValidator<OpinionFormDTO>
{
}
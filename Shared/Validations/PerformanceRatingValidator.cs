using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class PerformanceRatingBaseValidator<T> : AbstractValidator<T> where T : PerformanceRatingBase
{
    protected PerformanceRatingBaseValidator()
    {
        RuleFor(o => o.AppropriateAppealToPeople).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.Listening).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.FeedBack).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.Empathy).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.CommunicationObstacleRemove).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.NegativeNews).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(o => o.WorkInTeam).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.Leadership).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.ConflictResolution).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.MotivatePeople).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.StandUpForTeam).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(o => o.HumanValues).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.Fair).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.Altruism).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.LegalLiabilityAwareness).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.LegalLiabilityCompletion).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(o => o.WorkPlaceManagement).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.MeetingManagement).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.CrisisManagement).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.ManagementTechniquesApply).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        
        
        RuleFor(o => o.EmbraceLearningAndTeaching).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.TeachingEffort).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.ScientificThinking).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(o => o.HealthRiskAwareness).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.HealthProtectionVolunteer).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.FightAddiction).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.LifeStyleChangeRoleModel).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(o => o.SafetyProviding).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class PerformanceRatingValidator : PerformanceRatingBaseValidator<PerformanceRatingResponseDTO>
{
    public PerformanceRatingValidator()
    {
        

    }
}

public class PerformanceRatingDTOValidator : PerformanceRatingBaseValidator<PerformanceRatingDTO>
{
}
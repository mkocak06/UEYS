using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class PerformanceRatingBase
    {
        public long? Id { get; set; }
        // COMMUNİCATİON
        public int? AppropriateAppealToPeople { get; set; }
        public int? Listening { get; set; }
        public int? FeedBack { get; set; }
        public int? Empathy { get; set; }
        public int? CommunicationObstacleRemove { get; set; }
        public int? NegativeNews { get; set; }

        // MEMBERSHIP
        public int? WorkInTeam { get; set; }
        public int? Leadership { get; set; }
        public int? ConflictResolution { get; set; }
        public int? MotivatePeople { get; set; }
        public int? StandUpForTeam { get; set; }

        // RESPONSIBILITY
        public int? HumanValues { get; set; }
        public int? Fair { get; set; }
        public int? Altruism { get; set; }
        public int? LegalLiabilityAwareness { get; set; }
        public int? LegalLiabilityCompletion { get; set; }

        // MANAGEMENT
        public int? WorkPlaceManagement { get; set; }
        public int? MeetingManagement { get; set; }
        public int? CrisisManagement { get; set; }
        public int? ManagementTechniquesApply { get; set; }

        // LEARNER-TEACHER
        public int? EmbraceLearningAndTeaching { get; set; }
        public int? TeachingEffort { get; set; }
        public int? ScientificThinking { get; set; }

        // HEALTH PROTECTION
        public int? HealthRiskAwareness { get; set; }
        public int? HealthProtectionVolunteer { get; set; }
        public int? FightAddiction { get; set; }
        public int? LifeStyleChangeRoleModel { get; set; }
        public int? SafetyProviding { get; set; }

        public PeriodType? Period { get; set; }
        public RatingResultType? Result { get; set; }
        public DateTime? CreateDate { get; set; }

        public long? StudentId { get; set; }
        public long? EducatorId { get; set; }
    }
}

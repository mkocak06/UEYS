using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class PerformanceRatingResponseDTO : PerformanceRatingBase
    {


        public StudentResponseDTO Student { get; set; }
        public EducatorResponseDTO Educator { get; set; }
        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
        public string AverageCommunication => Shared.Extensions.EnumExtension.GetAverage(new int?[] { Ratings[0], Ratings[1], Ratings[2], Ratings[3], Ratings[4], Ratings[5] });
        public string AverageMemberShip => Shared.Extensions.EnumExtension.GetAverage(new int?[] { Ratings[6], Ratings[7], Ratings[8], Ratings[9], Ratings[10] });
        public string AverageResponsibility => Shared.Extensions.EnumExtension.GetAverage(new int?[] { Ratings[11], Ratings[12], Ratings[13], Ratings[14], Ratings[15] });
        public string AverageManagement => Shared.Extensions.EnumExtension.GetAverage(new int?[] { Ratings[16], Ratings[17], Ratings[18], Ratings[19] });
        public string AverageLearnerTeacher => Shared.Extensions.EnumExtension.GetAverage(new int?[] { Ratings[20], Ratings[21], Ratings[22] });
        public string AverageHealthProtection => Shared.Extensions.EnumExtension.GetAverage(new int?[] { Ratings[23], Ratings[24], Ratings[25], Ratings[26], Ratings[27] });
        public string OverallAverage => Shared.Extensions.EnumExtension.GetAverage(new int?[] {AppropriateAppealToPeople, Listening, FeedBack, Empathy,
                CommunicationObstacleRemove, NegativeNews, WorkInTeam, Leadership, ConflictResolution, MotivatePeople, StandUpForTeam,
                HumanValues, Fair, Altruism, LegalLiabilityAwareness, LegalLiabilityCompletion, WorkPlaceManagement, MeetingManagement, CrisisManagement,
                ManagementTechniquesApply, EmbraceLearningAndTeaching, TeachingEffort, ScientificThinking, HealthRiskAwareness, HealthProtectionVolunteer,
                FightAddiction, LifeStyleChangeRoleModel, SafetyProviding
                });
        public int?[] Ratings => new int?[] {AppropriateAppealToPeople, Listening, FeedBack, Empathy,
                CommunicationObstacleRemove, NegativeNews, WorkInTeam, Leadership, ConflictResolution, MotivatePeople, StandUpForTeam,
                HumanValues, Fair, Altruism, LegalLiabilityAwareness, LegalLiabilityCompletion, WorkPlaceManagement, MeetingManagement, CrisisManagement,
                ManagementTechniquesApply, EmbraceLearningAndTeaching, TeachingEffort, ScientificThinking, HealthRiskAwareness, HealthProtectionVolunteer,
                FightAddiction, LifeStyleChangeRoleModel, SafetyProviding
                };
    }
}

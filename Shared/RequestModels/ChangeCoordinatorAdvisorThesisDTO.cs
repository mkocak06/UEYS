using Shared.ResponseModels.Authorization;
using Shared.Types;
using System;

namespace Shared.RequestModels
{
    public class ChangeCoordinatorAdvisorThesisDTO
    {
        public DateTime? AdvisorAssignDate { get; set; }
        public long OldAdvisorId { get; set; }
        public bool IsCoordinator { get; set; }
        public string Description { get; set; }
        public long EducatorId { get; set; }
        public long ExpertiseBranchId { get; set; }
        public long ThesisId { get; set; }
        public long StudentId { get; set; }
        public long? UserId { get; set; }
        //public bool IsDeleted { get; set; }
        public ZoneStudentModel Zone { get; set; }
        public EducatorDeleteReasonType? DeleteReason { get; set; }
        public string DeleteExplanation { get; set; }

    }
}

using Shared.ResponseModels.Authorization;
using Shared.Types;
using System;

namespace Shared.BaseModels
{
    public class AdvisorThesisBase
    {
        public long Id { get; set; }
        public DateTime? AdvisorAssignDate { get; set; }
        public bool? IsCoordinator { get; set; }
        public string Description { get; set; }
        public long? EducatorId { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public long? ThesisId { get; set; }
        public long? UserId { get; set; }
        public bool IsDeleted { get; set; }
        //public ZoneStudentModel Zone { get; set; }
        public EducatorDeleteReasonType? DeleteReason { get; set; }
        public string DeleteExplanation { get; set; }
        public bool? ChangeCoordinator { get; set; }
        public bool? MakeCoordinator { get; set; }
        public long? StudentId { get; set; }
        public AdvisorType? Type { get; set; }

    }
}

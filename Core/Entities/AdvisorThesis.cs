using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AdvisorThesis : ExtendedBaseEntity
    {
        public DateTime? AdvisorAssignDate { get; set; }
        public bool? IsCoordinator { get; set; }
        public string Description { get; set; }
        public EducatorDeleteReasonType? DeleteReason { get; set; }
        public string DeleteExplanation { get; set; }

        public long? ExpertiseBranchId { get; set; }
        public virtual ExpertiseBranch ExpertiseBranch { get; set; }
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
        public long? ThesisId { get; set; }
        public virtual Thesis Thesis { get; set; }
        public long? UserId { get; set; }
        public virtual User User { get; set; }
        public AdvisorType? Type { get; set; }

    }
}

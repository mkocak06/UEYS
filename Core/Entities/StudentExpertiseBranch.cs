using System;

namespace Core.Entities
{
    public class StudentExpertiseBranch : BaseEntity
    {
        public DateTime? RegistrationDate { get; set; }

        public long? ExpertiseBranchId { get; set; }
        public virtual ExpertiseBranch ExpertiseBranch { get; set; }
        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}

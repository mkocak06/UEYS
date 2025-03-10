using System;

namespace Core.Entities
{
    public class EducatorExpertiseBranch : BaseEntity
    {
        public DateTime? RegistrationDate { get; set; }
        public string RegistrationNo { get; set; }
        public string RegistrationBranchName { get; set; }
        public string RegistrationGraduationSchool { get; set; }

        public long? ExpertiseBranchId { get; set; }
        public virtual ExpertiseBranch ExpertiseBranch { get; set; }
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
    }
}

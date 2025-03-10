using Core.Entities.Koru;
using System.Collections.Generic;

namespace Core.Entities
{
    public class ExpertiseBranch : BaseEntity
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public bool? IsPrincipal { get; set; }
        public int? ProtocolProgramCount { get; set; }
        public bool? IsIntensiveCare { get; set; }
        public int Code { get; protected set; }
        public int? PortfolioIndexRateToCapacityIndex { get; set; }
        public int? EducatorIndexRateToCapacityIndex { get; set; }
        public string CKYSCode { get; set; }
        public string SKRSCode { get; set; }

        public long? ProfessionId { get; set; }
        public virtual Profession Profession { get; set; }

        public virtual ICollection<RelatedExpertiseBranch> PrincipalBranches { get; set; }
        public virtual ICollection<RelatedExpertiseBranch> SubBranches { get; set; }

        public virtual ICollection<Rotation> Rotations { get; set; }
        public virtual ICollection<Program> Programs { get; set; }
        public virtual ICollection<EducatorExpertiseBranch> EducatorExpertiseBranches { get; set; }
        public virtual ICollection<StudentExpertiseBranch> StudentExpertiseBranches { get; set; }
        public virtual ICollection<Curriculum> Curricula { get; set; }
        public virtual ICollection<UserRoleFaculty> UserRoleFaculties { get; set; }
        public virtual ICollection<Portfolio> Portfolios { get; set; }
        public virtual ICollection<AdvisorThesis> AdvisorTheses { get; set; }
    }
}

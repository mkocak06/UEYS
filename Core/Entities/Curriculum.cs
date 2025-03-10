using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Curriculum : ExtendedBaseEntity
    {
        public bool IsActive { get; set; } = true;
        public string Version { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string DecisionNo { get; set; }
        public int? Duration { get; set; }

        public virtual ICollection<CurriculumRotation> CurriculumRotations { get; set; }
        public virtual ICollection<CurriculumPerfection> CurriculumPerfections { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Standard> Standards { get; set; }
        public virtual ICollection<SpecificEducation> SpecificEducations { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public virtual ExpertiseBranch ExpertiseBranch { get; set; }
    }
}

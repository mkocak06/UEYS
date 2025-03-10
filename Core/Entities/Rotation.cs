using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Rotation : ExtendedBaseEntity
    {
        public string Duration { get; set; }
        public bool? IsRequired { get; set; }

        public long? ExpertiseBranchId { get; set; }
        public virtual ExpertiseBranch ExpertiseBranch { get; set; }

        public virtual ICollection<Perfection> Perfections { get; set; }
        public virtual ICollection<StudentRotation> StudentRotations { get; set; }
        public virtual ICollection<CurriculumRotation> CurriculumRotations { get; set; }
    }
}
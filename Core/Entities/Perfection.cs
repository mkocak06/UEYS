using Shared.Types;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Perfection : ExtendedBaseEntity
    {
        public Perfection()
        {
            PerfectionProperties = new();
        }
        public PerfectionType PerfectionType { get; set; }
        public bool? IsRequired { get; set; }
        public string SpecialProvision { get; set; }
        public List<PerfectionProperty> PerfectionProperties { get; set; }

        public virtual long? CurriculumRotationId { get; set; }
        public virtual CurriculumRotation CurriculumRotation { get; set; }

        public virtual ICollection<StudentPerfection> StudentPerfections { get; set; }
        public virtual ICollection<StudentRotationPerfection> StudentRotationPerfections { get; set; }
        public virtual ICollection<CurriculumPerfection> CurriculumPerfections { get; set; }
    }
}

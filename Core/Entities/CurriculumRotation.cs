using System.Collections.Generic;

namespace Core.Entities
{
    public class CurriculumRotation : ExtendedBaseEntity
    {
        public long? CurriculumId { get; set; }
        public virtual Curriculum Curriculum { get; set; }
        public long? RotationId { get; set; }
        public virtual Rotation Rotation { get; set; }
        public virtual ICollection<Perfection> Perfections { get; set; }
    }
}

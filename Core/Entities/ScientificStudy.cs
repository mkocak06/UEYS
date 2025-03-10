using System;

namespace Core.Entities
{
    public class ScientificStudy : ExtendedBaseEntity
    {
        public DateTime? ProcessDate { get; set; }
        public string Description { get; set; }

        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }

        public long? StudyId { get; set; }
        public virtual Study Study { get; set; }
    }
}

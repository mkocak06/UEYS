using System;

namespace Core.Entities
{
    public class ProgressReport : ExtendedBaseEntity
    {
        public string Description { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public long? ThesisId { get; set; }
        public virtual Thesis Thesis { get; set; }
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
    }
}

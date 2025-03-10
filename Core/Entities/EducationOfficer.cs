using System;

namespace Core.Entities
{
    public class EducationOfficer : BaseEntity
    {
        public long? ProgramId { get; set; }
        public Program Program { get; set; }
        public long? EducatorId { get; set; }
        public Educator Educator { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DocumentOrder { get; set; }

    }
}

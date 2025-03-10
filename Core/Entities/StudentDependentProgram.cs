using System;

namespace Core.Entities
{
    public class StudentDependentProgram : BaseEntity
    {
        public bool? IsCompleted { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUnCompleted { get; set; }
        public int? RemainingDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Explanation { get; set; }

        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public long? DependentProgramId { get; set; }
        public virtual DependentProgram DependentProgram { get; set; }

    }
}

using System;

namespace Shared.BaseModels
{
    public class StudentDependentProgramBase
    {
        public bool? IsCompleted { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public long? StudentId { get; set; }
        public long? DependentProgramId { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class StudentRotation : ExtendedBaseEntity
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PreviousEndDate { get; set; }
        public DateTime? ProcessDateForExemption { get; set; }
        public bool? IsSuccessful { get; set; }
        public bool? IsUncompleted { get; set; }
        public int? RemainingDays { get; set; }
        public string EducatorName { get; set; }

        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public long? RotationId { get; set; }
        public virtual Rotation Rotation { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }

        public virtual ICollection<StudentRotationPerfection> StudentRotationPerfections { get; set; }
    }
}

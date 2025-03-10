using Shared.Types;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class ExitExam : ExtendedBaseEntity
    {
        public DateTime? ExamDate { get; set; }
        public double? PracticeExamNote { get; set; }
        public double? AbilityExamNote { get; set; }
        public string Description { get; set; }
        public ExitExamResultType? ExamStatus { get; set; }
        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public long? HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }
        public long? EducationTrackingId { get; set; }
        public virtual EducationTracking EducationTracking { get; set; }
        public long? SecretaryId { get; set; }
        public virtual User Secretary { get; set; }
        public virtual ICollection<Jury> Juries { get; set; }
    }
}

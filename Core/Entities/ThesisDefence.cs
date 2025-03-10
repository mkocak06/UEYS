using Shared.Types;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class ThesisDefence : ExtendedBaseEntity
    {
        public string Description { get; set; }
        public DateTime? ExamDate { get; set; }
        //public DateTime? ResultDate { get; set; }
        public DefenceResultType? Result { get; set; }
        public int? DefenceOrder { get; set; }

        public long? ThesisId { get; set; }
        public virtual Thesis Thesis { get; set; }
        public long? HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }
        public virtual ICollection<Jury> Juries { get; set; }
        public virtual ICollection<EducationTracking> EducationTrackings { get; set; }
    }
}

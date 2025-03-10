using Shared.Types;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class EducationTracking : ExtendedBaseEntity
    {
        public ProcessType? ProcessType { get; set; }
        public string Description { get; set; }
        public int? AdditionalDays { get; set; }
        public DateTime? ProcessDate { get; set; }
        public DateTime? SecondThesisDeadline { get; set; }
        public ReasonType? ReasonType { get; set; }
        public ExcusedType? ExcusedType { get; set; }
        public long? StudentRotationId { get; set; }
        public AssignmentType? AssignmentType { get; set; }
        public string PreviousDescription { get; set; }
        public int? PreviousAdditionalDays { get; set; }

        public long? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public long? ProcessOwnerId { get; set; }
        public virtual Educator ProcessOwner { get; set; }
        public long? FormerProgramId { get; set; }
        public virtual Program FormerProgram { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public long? RelatedEducationTrackingId { get; set; }
        public virtual EducationTracking RelatedEducationTracking { get; set; }
        public long? ThesisDefenceId { get; set; }
        public virtual ThesisDefence ThesisDefence{ get; set; }


        public virtual ICollection<ExitExam> ExitExams { get; set; }
    }
}

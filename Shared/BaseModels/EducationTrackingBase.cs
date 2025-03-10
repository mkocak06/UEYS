using Shared.Types;
using System;

namespace Shared.BaseModels
{
    public class EducationTrackingBase
    {
        public long? Id { get; set; }
        public ProcessType? ProcessType { get; set; }
        public string Description { get; set; }
        public int? AdditionalDays { get; set; }
        public DateTime? ProcessDate { get; set; }
        public DateTime? SecondThesisDeadline { get; set; }
        public DateTime? EndDate { get; set; }
        public ReasonType? ReasonType { get; set; }
        public ExcusedType? ExcusedType { get; set; }
        public long? StudentRotationId { get; set; }
        public AssignmentType? AssignmentType { get; set; }
        public int? PreviousAdditionalDays { get; set; }

        public long? ThesisDefenceId { get; set; }
        public long? StudentId { get; set; }
        public long? ProcessOwnerId { get; set; }
        public long? FormerProgramId { get; set; }
        public long? ProgramId { get; set; }
        public long? RelatedEducationTrackingId { get; set; }
    }
}

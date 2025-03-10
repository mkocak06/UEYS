using System;

namespace Shared.BaseModels
{
    public class StudentRotationBase
    {
        public long? Id { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PreviousEndDate { get; set; }
        public DateTime? ProcessDateForExemption { get; set; }
        public bool? IsSuccessful { get; set; }
        public bool? IsUncompleted { get; set; }
        public int? RemainingDays { get; set; }
        public string EducatorName { get; set; }

        public long? StudentId { get; set; }
        public long? RotationId { get; set; }
        public long? ProgramId { get; set; }
        public long? EducatorId { get; set; }
    }
}

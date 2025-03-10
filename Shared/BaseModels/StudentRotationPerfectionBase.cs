using System;

namespace Shared.BaseModels
{
    public class StudentRotationPerfectionBase
    {
        public DateTime? ProcessDate { get; set; }
        public bool? IsSuccessful { get; set; }
        public long? EducatorId { get; set; }
        public long? PerfectionId { get; set; }
        public long? StudentRotationId { get; set; }
    }
}

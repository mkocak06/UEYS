using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.BaseModels
{
    public class ExitExamBase
    {
        public long? Id { get; set; }

        public DateTime? ExamDate { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public double? PracticeExamNote { get; set; }
        public double? AbilityExamNote { get; set; }
        public ExitExamResultType? ExamStatus { get; set; }
        public string Description { get; set; }
        public bool? IsDeleted { get; set; }

        public long? StudentId { get; set; }
        public long? HospitalId { get; set; }
        public long? EducationTrackingId { get; set; }
        public long? SecretaryId { get; set; }

    }
}

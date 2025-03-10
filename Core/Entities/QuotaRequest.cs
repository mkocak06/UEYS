using Shared.Types;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class QuotaRequest : ExtendedBaseEntity
    {
        public YearPeriodType? Period { get; set; }
        public int? Year { get; set; }
        public PlacementExamType? Type { get; set; }
        public DateTime? TUKDecisionDate { get; set; }
        public string TUKDecisionNumber { get; set; }
        public DateTime? ApplicationStartDate { get; set; }
        public DateTime? ApplicationEndDate { get; set; }
        public bool? IsCompleted { get; set; }
        public string GlobalQuota { get; set; }
        public ICollection<SubQuotaRequest> SubQuotaRequests { get; set; }
    }
}

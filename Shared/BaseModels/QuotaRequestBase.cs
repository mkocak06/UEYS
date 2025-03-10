using Shared.Types;
using System.Collections.Generic;
using System;
using Shared.ResponseModels;

namespace Shared.BaseModels
{
    public class QuotaRequestBase
    {
        public YearPeriodType? Period { get; set; }
        public int? Year { get; set; }
        public PlacementExamType? Type { get; set; }
        public DateTime? TUKDecisionDate { get; set; }
        public string TUKDecisionNumber { get; set; }
        public DateTime? ApplicationStartDate { get; set; }
        public DateTime? ApplicationEndDate { get; set; }
        public List<GlobalQuotaExpertiseBranchModel> GlobalQuota { get; set; }
        public bool? IsCompleted { get; set; }
    }
}

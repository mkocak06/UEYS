using Shared.ResponseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class GlobalQuotaExpertiseBranchModel
    {
        public long? ExpertiseBranchId { get; set; }
        public string ExpertiseBranchName { get; set; }
        public int? AnnualGlobalQuota { get; set; }
        public int? FirstPeriodGlobalQuota { get; set; }
        public int? SecondPeriodGlobalQuota { get; set; }
    }
}

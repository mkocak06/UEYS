using System.Collections.Generic;

namespace Core.Entities
{
    public class Portfolio : ExtendedBaseEntity
    {
        public string Name { get; set; }
        public int? Ratio { get; set; }

        public long? ExpertiseBranchId { get; set; }
        public ExpertiseBranch ExpertiseBranch { get; set; }
        public virtual ICollection<SubQuotaRequestPortfolio> SubQuotaRequestPortfolios { get; set; }

    }
}

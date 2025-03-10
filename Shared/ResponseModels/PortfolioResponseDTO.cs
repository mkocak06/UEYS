using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class PortfolioResponseDTO : PortfolioBase
    {
        public long? Id { get; set; }
        public ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
        public virtual List<SubQuotaRequestPortfolioResponseDTO> SubQuotaRequestPortfolios { get; set; }
    }
}

using System.Collections.Generic;
using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class SubQuotaRequestDTO : SubQuotaRequestBase
    {
        public QuotaRequestDTO QuotaRequest { get; set; }
        public virtual ICollection<SubQuotaRequestPortfolioDTO> SubQuotaRequestPortfolios { get; set; }
        public virtual ICollection<StudentCountDTO> StudentCounts { get; set; }
    }
}

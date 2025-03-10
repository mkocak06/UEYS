using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class SubQuotaRequestResponseDTO : SubQuotaRequestBase
    {
        public long? Id { get; set; }
        public ProgramResponseDTO Program { get; set; }
        public QuotaRequestResponseDTO QuotaRequest { get; set; }
        public virtual ICollection<SubQuotaRequestPortfolioResponseDTO> SubQuotaRequestPortfolios { get; set; }
        public List<StudentCountResponseDTO> StudentCounts { get; set; }

    }
}

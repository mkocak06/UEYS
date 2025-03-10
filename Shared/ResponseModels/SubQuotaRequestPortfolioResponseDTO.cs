using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class SubQuotaRequestPortfolioResponseDTO : SubQuotaRequestPortfolioBase
    {
        public long? Id { get; set; }
        public PortfolioResponseDTO Portfolio { get; set; }
        public SubQuotaRequestResponseDTO SubQuotaRequest { get; set; }
    }
}

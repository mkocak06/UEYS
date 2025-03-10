using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class SubQuotaRequestPortfolioResponseDTO_2
    {
        public long? Id { get; set; }
        public long? PortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public long? SubQuotaRequestId { get; set; }
        public int? Answer { get; set; }
        public int? Ratio { get; set;}
    }
}

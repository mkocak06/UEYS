namespace Shared.BaseModels
{
    public class PortfolioBase
    {
        public string Name { get; set; }
        public int? Ratio { get; set; }

        public long? ExpertiseBranchId { get; set; }
    }
}

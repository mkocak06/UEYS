namespace Shared.ResponseModels.Standard
{
    public class ProgramStandardResponseDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ExpertiseBranchName { get; set; }
        public long ExpertiseBranchId { get; set; }
        public string ExpertiseBranchVersion { get; set; }
        public string StandardCategory { get; set; }
        public string Description { get; set; }
    }
}

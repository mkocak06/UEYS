using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class RelatedExpertiseBranchResponseDTO : RelatedExpertiseBranchBase
    {
        public ExpertiseBranchResponseDTO PrincipalBranch { get; set; }
        public ExpertiseBranchResponseDTO SubBranch { get; set; }
    }
}

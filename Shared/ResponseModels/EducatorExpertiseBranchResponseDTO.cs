using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class EducatorExpertiseBranchResponseDTO : EducatorExpertiseBranchBase
    {
        public virtual ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
    }
}

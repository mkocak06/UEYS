using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class StudentExpertiseBranchResponseDTO : StudentExpertiseBranchBase
    {
        public virtual ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
    }
}

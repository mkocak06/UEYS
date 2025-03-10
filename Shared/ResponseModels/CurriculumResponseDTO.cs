using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class CurriculumResponseDTO : CurriculumBase
    {
        public long Id { get; set; }

        public virtual ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }

    }
}

using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class AdvisorThesisResponseDTO : AdvisorThesisBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
        public ThesisResponseDTO Thesis { get; set; }
        public UserResponseDTO User { get; set; }
    }
}

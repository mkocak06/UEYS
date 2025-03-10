using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class JuryResponseDTO : JuryBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public ThesisDefenceResponseDTO ThesisDefence { get; set; }
        public UserResponseDTO User { get; set; }
        public ExitExamResponseDTO ExitExam { get; set; }
    }
}

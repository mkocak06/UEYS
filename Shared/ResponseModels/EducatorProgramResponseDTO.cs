using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class EducatorProgramResponseDTO : EducatorProgramBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public ProgramResponseDTO Program { get; set; }
    }
}

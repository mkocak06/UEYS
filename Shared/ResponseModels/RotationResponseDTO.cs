using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class RotationResponseDTO : RotationBase
    {
        public long? Id { get; set; }
        public ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
        public List<StudentRotationResponseDTO> StudentRotations { get; set; }
        public List<CurriculumRotationResponseDTO> CurriculumRotations { get; set; }
        public List<PerfectionResponseDTO> Perfections { get; set; }
    }
}

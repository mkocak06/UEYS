using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class CurriculumRotationResponseDTO : CurriculumRotationBase
    {
        public long? Id { get; set; }
        public virtual CurriculumResponseDTO Curriculum { get; set; }
        public virtual RotationResponseDTO Rotation { get; set; }
        public List<PerfectionResponseDTO> Perfections { get; set; }
    }
}

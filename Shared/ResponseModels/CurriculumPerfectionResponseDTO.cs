using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class CurriculumPerfectionResponseDTO : CurriculumPerfectionBase
    {
        public long? Id { get; set; }
        public virtual CurriculumResponseDTO Curriculum { get; set; }
        public virtual PerfectionResponseDTO Perfection { get; set; }
    }
}

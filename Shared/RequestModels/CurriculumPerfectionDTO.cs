using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class CurriculumPerfectionDTO : CurriculumPerfectionBase
    {
        public PerfectionDTO Perfection { get; set; }
    }
}

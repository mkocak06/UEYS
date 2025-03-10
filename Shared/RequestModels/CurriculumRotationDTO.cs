using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class CurriculumRotationDTO : CurriculumRotationBase
    {
        public RotationDTO Rotation { get; set; }
    }
}

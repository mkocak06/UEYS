using Shared.BaseModels;

namespace Shared.ResponseModels.Standard
{
    public class ProgramFormStandardResponseDTO : FormStandardBase
    {
        public virtual long Id { get; set; }
        public virtual ProgramStandardResponseDTO Standard { get; set; }
        public virtual FormResponseDTO Form { get; set; }
    }
}

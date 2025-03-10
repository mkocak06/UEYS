using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class FormStandardResponseDTO : FormStandardBase
    {
        public virtual long Id { get;  set; }
        public virtual StandardResponseDTO Standard { get; set; }
        public virtual FormResponseDTO Form { get; set; }
    }
}

using Shared.BaseModels;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class ThesisDefenceDTO : ThesisDefenceBase
    {
        public virtual ICollection<JuryDTO> Juries { get; set; }
    }
}

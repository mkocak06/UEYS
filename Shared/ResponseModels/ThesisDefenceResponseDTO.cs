using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class ThesisDefenceResponseDTO : ThesisDefenceBase
    {
        public virtual ThesisResponseDTO Thesis { get; set; }
        public virtual HospitalResponseDTO Hospital { get; set; }

        public virtual List<JuryResponseDTO> Juries { get; set; }
    }
}

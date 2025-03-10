using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class EthicCommitteeDecisionResponseDTO : EthicCommitteeDecisionBase
    {
        public ThesisResponseDTO Thesis { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
    }
}

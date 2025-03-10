using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class EthicCommitteeDecisionDTO : EthicCommitteeDecisionBase
    {
        public virtual DocumentDTO Document { get; set; }
    }
}

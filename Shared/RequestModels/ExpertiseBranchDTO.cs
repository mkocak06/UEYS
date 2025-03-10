using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class ExpertiseBranchDTO : ExpertiseBranchBase
    {
        public virtual List<RelatedExpertiseBranchDTO> SubBranches { get; set; }
        public virtual List<RelatedExpertiseBranchDTO> PrincipalBranches { get; set; }
    }
}

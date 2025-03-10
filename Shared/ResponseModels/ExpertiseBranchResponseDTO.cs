using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class ExpertiseBranchResponseDTO : ExpertiseBranchBase
    {
        public virtual ProfessionResponseDTO Profession  { get; set; }
        public virtual List<RelatedExpertiseBranchResponseDTO> PrincipalBranches { get; set; }
        public virtual List<RelatedExpertiseBranchResponseDTO> SubBranches { get; set; }
        public virtual List<PortfolioResponseDTO> Portfolios { get; set; }
    }
}

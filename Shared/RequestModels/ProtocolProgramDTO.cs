using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestModels
{
    public class ProtocolProgramDTO : ProtocolProgramBase
    {
        public virtual List<RelatedDependentProgramDTO> RelatedDependentPrograms { get; set; }
    }
}

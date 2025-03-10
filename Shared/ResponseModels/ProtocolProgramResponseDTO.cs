using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class ProtocolProgramResponseDTO : ProtocolProgramBase
    {
        public virtual ProgramResponseDTO ParentProgram { get; set; }
        public virtual List<RelatedDependentProgramResponseDTO> RelatedDependentPrograms { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public virtual List<ExpertiseBranchResponseDTO> MissingExpertiseBranches{ get; set; }

    }
}

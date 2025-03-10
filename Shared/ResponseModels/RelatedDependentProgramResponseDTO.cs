using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class RelatedDependentProgramResponseDTO : RelatedDependentProgramBase
    {
        public ProtocolProgramResponseDTO ProtocolProgram { get; set; }
        public virtual List<DependentProgramResponseDTO> ChildPrograms { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
    }
}

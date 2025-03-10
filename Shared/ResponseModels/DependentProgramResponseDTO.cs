using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class DependentProgramResponseDTO: DependentProgramBase
    {
        public virtual ProgramResponseDTO Program { get; set; }
        public virtual RelatedDependentProgramResponseDTO RelatedDependentProgram { get; set; }
        public virtual List<EducatorDependentProgramResponseDTO> EducatorDependentPrograms { get; set; }
    }
}

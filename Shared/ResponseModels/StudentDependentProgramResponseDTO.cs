using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class StudentDependentProgramResponseDTO : StudentDependentProgramBase
    {
        public virtual StudentResponseDTO Student{ get; set; }
        public virtual DependentProgramResponseDTO DependentProgram{ get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.ProtocolProgram
{
    public class StudentDependentProgramsDTO
    {
        public List<StudentDependentProgramPaginateDTO> DependentPrograms { get; set; }
    }
}

using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class EducatorDependentProgramResponseDTO : EducatorDependentProgramBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public DependentProgramResponseDTO DependentProgram { get; set; }
        public string AcademicTitleName { get; set; }
        public string StaffTitleName { get; set; }
        public string EducatorName { get; set; }
        public string Phone { get; set; }
    }
}

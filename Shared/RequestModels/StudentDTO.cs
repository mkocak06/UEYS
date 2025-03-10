using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class StudentDTO : StudentBase
    {
        public virtual List<EducationTrackingDTO> EducationTrackings { get; set; }
        public virtual List<StudentExpertiseBranchDTO> StudentExpertiseBranches { get; set; }
        public virtual List<StudentDependentProgramDTO> StudentDependentPrograms{ get; set; }
        public virtual UpdateUserAccountInfoDTO User { get; set; }
    }
}

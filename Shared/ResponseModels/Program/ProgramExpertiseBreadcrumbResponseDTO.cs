using Shared.ResponseModels.University;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Program
{
    public class ProgramExpertiseBreadcrumbResponseDTO
    {
        public long Id{ get; set; }
        public string HospitalName{ get; set; }
        public string ExpertiseBranchName { get; set; }
    }
}

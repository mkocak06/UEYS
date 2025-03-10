using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.ProtocolProgram
{
    public class ProtocolProgramByUniversityIdResponseDTO
    {
        public long Id { get; set; }
        public string ExpertiseBranch { get; set; }
        public virtual ICollection<string> ChildProgramNames { get; set; }
        public string UniversityName { get; set; }
        public string FacultyName { get; set; }
        public string HospitalName { get; set; }
        public string ManagerName { get; set; }
    }
}

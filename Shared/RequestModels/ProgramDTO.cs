using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class ProgramDTO : ProgramBase
    {
        public virtual FacultyDTO Faculty { get; set; }
        public virtual ICollection<EducatorProgramDTO> EducatorPrograms { get; set; }
        public virtual ICollection<AuthorizationDetailDTO> AuthorizationDetails { get; set; }
    }
}

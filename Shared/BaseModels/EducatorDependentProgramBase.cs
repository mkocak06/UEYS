using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class EducatorDependentProgramBase
    {
        public long? Id { get; set; }
        public bool? IsProgramManager { get; set; }

        public long? EducatorId{ get; set; }
        public long? DependentProgramId { get; set; }
    }
}

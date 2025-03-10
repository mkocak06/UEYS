using Shared.BaseModels;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestModels
{
    public class DependentProgramDTO : DependentProgramBase
    {
        public virtual ICollection<EducatorDependentProgramDTO> EducatorDependentPrograms { get; set; }
    }
}

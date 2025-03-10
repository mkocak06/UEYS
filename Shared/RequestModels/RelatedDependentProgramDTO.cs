using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class RelatedDependentProgramDTO : RelatedDependentProgramBase
    {
        public virtual List<DependentProgramDTO> ChildPrograms{ get; set; }
    }
}

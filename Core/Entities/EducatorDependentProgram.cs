using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EducatorDependentProgram : BaseEntity
    {
        public bool? IsProgramManager { get; set; }

        public long? EducatorId { get; set; }
        public virtual Educator Educator { get; set; }
        public long? DependentProgramId { get; set; }
        public virtual DependentProgram DependentProgram{ get; set; }
    }
}

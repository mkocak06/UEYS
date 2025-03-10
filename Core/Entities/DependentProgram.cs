using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DependentProgram : BaseEntity
    {
        public bool? IsAdministratorProgram { get; set; }
        public int? Duration { get; set; }
        public string Unit { get; set; }

        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public long? RelatedDependentProgramId { get; set; }
        public virtual RelatedDependentProgram RelatedDependentProgram { get; set; }

        public virtual ICollection<EducatorDependentProgram> EducatorDependentPrograms { get; set; }
        public virtual ICollection<StudentDependentProgram> StudentDependentPrograms { get; set; }
    }
}

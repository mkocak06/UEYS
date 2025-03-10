using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class DependentProgramBase
    {
        public long Id { get; set; }
        public bool? IsAdministratorProgram { get; set; }
        public int? Duration { get; set; }
        public string Unit { get; set; }

        public long? ProgramId { get; set; }
        public long? RelatedDependentProgramId { get; set; }

    }
}

using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProtocolProgram : ExtendedBaseEntity
    {
        public string ProtocolNo { get; set; }
        public ProgramType Type { get; set; }
        public string DecisionNo { get; set; }
        public DateTime? DecisionDate { get; set; }
        public DateTime? CancelingDate { get; set; }
        public string CancelingProtocolNo { get; set; }
        public long? ParentProgramId { get; set; }
        public virtual Program ParentProgram { get; set; }

        public virtual ICollection<RelatedDependentProgram> RelatedDependentPrograms { get; set; }
    }
}

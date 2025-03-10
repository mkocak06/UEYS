using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RelatedDependentProgram : BaseEntity
    {
        public bool IsActive { get; set; } = false;
        public int? Revision { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string DecisionNo { get; set; }

        public long ProtocolProgramId { get; set; }
        public ProtocolProgram ProtocolProgram { get; set; }

        public virtual ICollection<DependentProgram> ChildPrograms { get; set; }

        public long? DocumentId { get; set; }
    }
}

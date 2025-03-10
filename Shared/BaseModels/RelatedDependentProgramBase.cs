using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class RelatedDependentProgramBase
    {
        public long? Id { get; set; }
        public bool IsActive { get; set; }
        public int? Revision { get; set; }
        public long ProtocolProgramId { get; set; }
         public DateTime? DecisionDate { get; set; }
        public string DecisionNo { get; set; }
        public long? DocumentId { get; set; }

    }
}

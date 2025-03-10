using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class ProtocolProgramBase
    {
        public long Id { get; set; }
        public string ProtocolNo { get; set; }
        public ProgramType Type{ get; set; }
        public string DecisionNo { get; set; }
        public DateTime? DecisionDate { get; set; }
        public DateTime? CancelingDate { get; set; }
        public string CancelingProtocolNo { get; set; }
        public long? ParentProgramId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

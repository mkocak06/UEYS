using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.ProtocolProgram
{
    public class ProtocolProgramPaginatedResponseDTO
    {
        public long Id { get; set; }
        public string ProtocolNo { get; set; }
        public string Province { get; set; }
        public string Faculty { get; set; }
        public string Hospital { get; set; }
        public string ExpertiseBranch { get; set; }
        public int RelatedProgramsCount { get; set; }
        public string CancelingProtocolNo { get; set; }
        public int? RelatedRevision { get; set; }
    }
}


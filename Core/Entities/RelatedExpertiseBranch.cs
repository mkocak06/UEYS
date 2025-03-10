using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RelatedExpertiseBranch
    {
        public long PrincipalBranchId { get; set; }
        public ExpertiseBranch PrincipalBranch { get; set; }
        public long SubBranchId { get; set; }
        public ExpertiseBranch SubBranch { get; set; }
    }
}

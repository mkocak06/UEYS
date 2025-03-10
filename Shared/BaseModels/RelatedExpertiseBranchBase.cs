using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class RelatedExpertiseBranchBase
    {
        public long? PrincipalBranchId { get; set; }
        public long? SubBranchId { get; set; }
    }
}

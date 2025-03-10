using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Authorization
{
    public class ZoneRegisterDTO
    {
        public long RoleId { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public List<long> ZoneIds { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class UserRoleBase
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public List<long> RoleIds { get; set; }
    }
}

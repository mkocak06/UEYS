using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class UserRoleProvinceBase
    {
        public long? Id { get; set; }
        public long? UserRoleId { get; set; }
        public long? ProvinceId { get; set; }
    }
}

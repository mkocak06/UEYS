using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class UserRoleHospitalBase
    {
        public long? Id { get; set; }
        public long? UserRoleId { get; set; }
        public long? HospitalId { get; set; }
    }
}

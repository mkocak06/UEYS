using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class UserRoleProvinceResponseDTO : UserRoleProvinceBase
    {
        public UserRoleResponseDTO UserRole { get; set; }
        public ProvinceResponseDTO Province { get; set; }
    }
}

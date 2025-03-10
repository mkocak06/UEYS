using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class UserSettingResponseDTO
    {
        public long Id { get; set; }
        public long SelectedRoleId { get; set; }
        public RoleResponseDTO Role { get; set; }
    }
}

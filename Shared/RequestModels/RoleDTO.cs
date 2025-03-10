using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class RoleDTO : RoleBase
    {
        public List<PermissionDTO> Permissions { get; set; }
        public List<MenuDTO> Menus { get; set; }
        public List<RolePermission2DTO> RolePermissions { get; set; }
    }
}

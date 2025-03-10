using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;
using Shared.ResponseModels.Menu;

namespace Shared.ResponseModels
{
    public class RoleResponseDTO : RoleBase
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public bool? IsAutomated { get; set; }
        public List<PermissionResponseDTO> Permissions { get; set; }
        public List<MenuResponseDTO> Menus { get; set; }

        public static implicit operator List<object>(RoleResponseDTO v)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class RolePermissionResponseDTO : RolePermissionBase
    {
        public long PermissionId { get; set; }
        public PermissionResponseDTO Permission { get; set; }
    }
}

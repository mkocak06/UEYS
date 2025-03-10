using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class PermissionResponseDTO : PermissionBase
    {
        public string Group { get; set; }
        public bool IsActive { get; set; }
        public long RoleId { get; set; }
    }
}

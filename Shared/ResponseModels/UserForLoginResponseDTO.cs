using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class UserForLoginResponseDTO : UserForLoginBase
    {
        public long Id { get; set; }
        public long SelectedRoleId { get; set; }
        public List<long> UserRoleIds { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string ProfilePhoto { get; set; }
        public RoleCategoryType? RoleCategoryType { get; set; }
    }
}
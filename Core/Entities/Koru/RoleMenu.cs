using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Koru
{
    public class RoleMenu : BaseEntity
    {
        public long? MenuId { get; set; }
        public virtual Menu Menu { get; set; }
        public long? RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}

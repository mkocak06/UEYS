using Core.Entities.Koru;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Menu : BaseEntity
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public int Order { get; set; }
        public long? ParentId { get; set; }
        public virtual Menu ParentMenu { get; set; }
        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
        public virtual ICollection<Menu> ChildMenus { get; set; }
    }
}

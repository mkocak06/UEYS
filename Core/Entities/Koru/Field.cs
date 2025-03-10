using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Koru
{
    public class Field : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string EntityName { get; set; }
        public long? PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
        public virtual ICollection<RoleField> RoleFields { get; set; }
    }
}

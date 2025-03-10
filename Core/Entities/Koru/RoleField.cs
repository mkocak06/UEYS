using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Koru
{
    public class RoleField : BaseEntity
    {
        public long? RoleId { get; set; }
        public virtual Role Role { get; set; }
        public long? FieldId { get; set; }
        public virtual Field Field { get; set; }
    }
}

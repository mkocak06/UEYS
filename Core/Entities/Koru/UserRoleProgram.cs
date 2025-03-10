using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Koru
{
    public class UserRoleProgram : BaseEntity
    {
        public long? UserRoleId { get; set; }
        public virtual UserRole UserRole { get; set; }
        public long? ProgramId { get; set; }
        public virtual Program Program { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}

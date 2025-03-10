using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Koru
{
    public class UserRoleFaculty : BaseEntity
    {
        public long? UserRoleId { get; set; }
        public virtual UserRole UserRole { get; set; }
        public long? FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public virtual ExpertiseBranch ExpertiseBranch { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}

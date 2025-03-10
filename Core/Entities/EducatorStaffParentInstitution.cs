using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EducatorStaffParentInstitution : BaseEntity
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public long? EducatorId { get; set; }
        public Educator Educator { get; set; }
        public long? StaffParentInstitutionId { get; set; }
        public University StaffParentInstitution { get; set; }
    }
}

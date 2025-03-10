using System;

namespace Core.Entities
{
    public class EducatorStaffInstitution : BaseEntity
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public long? EducatorId { get; set; }
        public Educator Educator { get; set; }
        public long? StaffInstitutionId { get; set; }
        public Hospital StaffInstitution { get; set; }
    }
}

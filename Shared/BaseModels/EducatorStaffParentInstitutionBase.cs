using System;

namespace Shared.BaseModels
{
    public class EducatorStaffParentInstitutionBase
    {
        public long? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public long? EducatorId { get; set; }
        public long? StaffParentInstitutionId { get; set; }
    }
}

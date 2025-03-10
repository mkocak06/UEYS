using System;

namespace Shared.BaseModels
{
    public class EducatorStaffInstitutionBase
    {
        public long? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public long? EducatorId { get; set; }
        public long? StaffInstitutionId { get; set; }
    }
}

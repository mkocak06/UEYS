using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class EducatorStaffParentInstitutionResponseDTO : EducatorStaffParentInstitutionBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public UniversityResponseDTO StaffParentInstitution { get; set; }
    }
}

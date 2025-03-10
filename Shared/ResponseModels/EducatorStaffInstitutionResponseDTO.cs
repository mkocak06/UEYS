using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class EducatorStaffInstitutionResponseDTO : EducatorStaffInstitutionBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public HospitalResponseDTO StaffInstitution { get; set; }
    }
}

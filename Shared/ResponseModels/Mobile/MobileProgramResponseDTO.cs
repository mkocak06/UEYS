using System.Collections.Generic;

namespace Shared.ResponseModels.Mobile
{
    public class MobileProgramResponseDTO
    {
        public long? UniversityId { get; set; }
        public string UniversityName { get; set; }
        public long? FacultyId { get; set; }
        public string FacultyName { get; set; }
        public long? HospitalId { get; set; }
        public string HospitalName { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public string ExpertiseBranchName { get; set; }
        public virtual ICollection<MobileAuthDetailResponseDTO> AuthorizationDetails { get; set; }
    }
}

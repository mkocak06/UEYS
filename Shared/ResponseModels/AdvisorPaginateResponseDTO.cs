using Shared.BaseModels;
using Shared.Types;
using System.Collections.Generic;
using System.Security.Principal;

namespace Shared.ResponseModels
{
    public class AdvisorPaginateResponseDTO
    {
        public long Id { get; set; }
        public EducatorType Type { get; set; }
        public string AcademicTitle { get; set; }
        public string DutyPlaceHospital { get; set; }
        public long? DutyPlaceHospitalId { get; set; }
        public IEnumerable<EducatorExpertiseBranchResponseDTO> ExpertiseBranches { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }
        public bool UserIsDeleted { get; set; }
        public long UserId { get; set; }
        public string IdentityNo { get; set; }
        //public bool IsExistEducationPlace { get; set; }
        public IEnumerable<long?> ExpertiseBranchIds { get; set; }
    }
}

using Shared.Types;
using System.Collections.Generic;

namespace Shared.ResponseModels.Mobile
{
    public class MobileEducatorPaginateResponseDTO
    {
        public long? Id { get; set; }
        public string IdentityNo { get; set; }
        public string Name { get; set; }
        public string AcademicTitle { get; set; }
        public string StaffTitle { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PrincipalBranchName { get; set; }
        public string PrincipalBranchDutyPlace { get; set; }
        public string PrincipalBranchDuty { get; set; }
        public long? PrincipalBranchDutyPlaceId { get; set; }
        public long? PrincipalBranchDutyPlaceHospitalId { get; set; }
        public long? PrincipalBranchDutyPlaceUniversityId { get; set; }
        public int? PrincipalBranchDutyType { get; set; }
		public string SubBranchName { get; set; }
		public string SubBranchDutyPlace { get; set; }
        public long? SubBranchDutyPlaceId { get; set; }
        public long? SubBranchDutyPlaceHospitalId { get; set; }
        public long? SubBranchDutyPlaceUniversityId { get; set; }
        public int? SubBranchDutyType{ get; set; }
        public bool? IsDeleted { get; set; }
        public bool? UserIsDeleted { get; set; }
        public EducatorType EducatorType { get; set; }
        public EducatorDeleteReasonType? DeleteReason{ get; set; }
        public string DeleteReasonExplanation{ get; set; }
        public List<string> Roles { get; set; }
        public List<string> EducatorAdministrativeTitles { get; set; }
        public bool? IsConditionalEducator { get; set; }
        public List<string> ExpertiseBranches { get; set; }
        public long? EducationOfficerProgramId { get; set; }
        public long? EducationOfficerId { get; set; }

    }
}

using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels.Mobile
{
    public class MobileProgramPaginateResponseDTO
    {
        public long Id { get; set; }

        public string ProvinceName { get; set; }
        public long? ProvinceId { get; set; }
        public string ParentInstitutionName { get; set; }
        public long? ParentInstitutionId { get; set; }
        public string ProfessionName { get; set; }
        public long? ProfessionId { get; set; }
        public string UniversityName { get; set; }
        public long? UniversityId { get; set; }
        public string FacultyName { get; set; }
        public long? FacultyId { get; set; }
        public string HospitalName { get; set; }
        public long? HospitalId { get; set; }
        public string ExpertiseBranchName { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public DateTime? AuthorizationEndDate { get; set; }
        public DateTime? AuthorizationVisitDate { get; set; }
        public DateTime? AuthorizationDecisionDate { get; set; }
        public string AuthorizationDecisionNo { get; set; }
        public string AuthorizationCategory { get; set; }
        public bool? AuthorizationCategoryIsActive { get; set; }
        public string AuthorizationCategoryColorCode { get; set; }
        public long? AuthorizationCategoryId { get; set; }
        public bool? IsPrincipal { get; set; }
        public string AffiliationProtocolNo { get; set; }
        public string AffiliationHospitalName { get; set; }
        public string AffiliationUniversityName { get; set; }
        public bool? IsDeleted { get; set; }
        public List<long> ProtocolProgramIdList { get; set; }
        public long? AffiliationId { get; set; }
        public List<ProgramType> ProgramType { get; set; }
        public bool? CanMakeProtocol{ get; set; }
        public List<long> PrincipalBrancIdList { get; set; }
    }
}

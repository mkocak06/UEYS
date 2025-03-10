using Shared.Types;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Shared.ResponseModels.Program
{
    public class ProgramExportResponseDTO
    {
        public long Id { get; set; }
        public string ProvinceName { get; set; }
        public string ParentInstitutionName { get; set; }
        public string ProfessionName { get; set; }
        public string UniversityName { get; set; }
        public string FacultyName { get; set; }
        public string HospitalName { get; set; }
        public string ExpertiseBranchName { get; set; }
        public string AffiliatedUniversityName { get; set; }
        public string AffiliatedFacultyName { get; set; }
        public DateTime? AuthorizationEndDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsPrivate { get; set; }
        public string AuthorizationCategory { get; set; }
        public int? StudentCount { get; set; }
        public bool? IsPrincipal { get; set; }
        public string ProtocolStatus { get; set; }

        public List<AuthorizationDetailExportResponseDTO> AuthorizationDetails { get; set; }
    }
}

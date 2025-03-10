using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels.Program
{
    public class ProgramPaginateForQuotaResponseDTO
    {
        public long Id { get; set; }

        public long? ProfessionCode { get; set; }
        public string ProgramName { get; set; }
        public string HospitalName { get; set; }
        public string ExpertiseBranchName { get; set; }
        public string ProfessionName { get; set; }
        public string AuthorizationCategory { get; set; }
        public bool? IsQuotaRequestable{ get; set; }
        public bool? IsPrincipal { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ProfessorCount { get; set; }
        public int? EducationPersonCount { get; set; }
        public int? AssociateProfessorCount { get; set; }
        public int? DoctorLecturerCount { get; set; }
        public int? ChiefAssistantCount { get; set; }
        public int? TotalEducatorCount { get; set; }
        public int? CurrentStudentCount { get; set; }
        public int? StudentWhoLast6MonthToFinishCount { get; set; }
        public List<PortfolioResponseDTO> Portfolios { get; set; }
    }
}

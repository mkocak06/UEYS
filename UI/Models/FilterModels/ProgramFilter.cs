using Shared.ResponseModels;
using System;
using System.Collections.Generic;

namespace UI.Models.FilterModels
{
    public class ProgramFilter
    {
        public IList<ProvinceResponseDTO> ProvinceList { get; set; }
        public IList<InstitutionResponseDTO> InstitutionList { get; set; }
        public IList<UniversityResponseDTO> UniversityList { get; set; }
        public IList<FacultyResponseDTO> FacultyList { get; set; }
        public IList<ProfessionResponseDTO> ProfessionList { get; set; }
        public IList<HospitalResponseDTO> HospitalList { get; set; }
        public IList<ExpertiseBranchResponseDTO> ExpertiseBranchList { get; set; }
        public IList<AuthorizationCategoryResponseDTO> AuthorizationCategoryList { get; set; }
        public IList<bool?> UniversityTypeList { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsPrincipal { get; set; }
        public bool? IsPrivate { get; set; }
    }
}

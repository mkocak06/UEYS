using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class EducatorResponseDTO : EducatorBase
    {
        public virtual TitleResponseDTO StaffTitle { get; set; }
        public virtual TitleResponseDTO AcademicTitle { get; set; }

        public virtual UserAccountDetailInfoResponseDTO User { get; set; }
        public List<EducatorProgramResponseDTO> EducatorPrograms { get; set; }
        public List<EducationOfficerResponseDTO> EducationOfficers { get; set; }
        public List<EducatorExpertiseBranchResponseDTO> EducatorExpertiseBranches { get; set; }
        public IList<EducatorAdministrativeTitleResponseDTO> EducatorAdministrativeTitles { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public virtual List<GraduationDetailResponseDTO> GraduationDetails { get; set; }
        public virtual List<EducatorStaffInstitutionResponseDTO> StaffInstitutions { get; set; }
        public virtual List<EducatorStaffParentInstitutionResponseDTO> StaffParentInstitutions { get; set; }
    }
}

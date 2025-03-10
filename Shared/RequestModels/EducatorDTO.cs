using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class EducatorDTO : EducatorBase
    {
        public virtual List<EducatorProgramDTO> EducatorPrograms { get; set; }
        public virtual List<EducationOfficerDTO> EducationOfficers{ get; set; }
        public virtual List<EducatorExpertiseBranchDTO> EducatorExpertiseBranches { get; set; }
        public virtual List<EducatorAdministrativeTitleDTO> EducatorAdministrativeTitles { get; set; }
        public virtual UpdateUserAccountInfoDTO User { get; set; }
        public virtual List<DocumentDTO> Documents { get; set; }
        public virtual List<GraduationDetailDTO> GraduationDetails { get; set; }
        public virtual List<EducatorStaffParentInstitutionDTO> StaffParentInstitutions { get; set; }
        public virtual List<EducatorStaffInstitutionDTO> StaffInstitutions { get; set; }
    }
}

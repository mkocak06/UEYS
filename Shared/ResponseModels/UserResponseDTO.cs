using Shared.BaseModels;
using Shared.Models;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class UserResponseDTO : UserBase
    {
        public virtual InstitutionResponseDTO Institution { get; set; }
        public virtual CountryResponseDTO Country { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public virtual List<UserRoleResponseDTO> UserRoles { get; set; }
        public virtual EducatorResponseDTO Educator { get; set; }
        public virtual StudentResponseDTO Student { get; set; }
        public virtual CKYSDoctor CKYSDoctorResult { get; set; }
        public virtual AcademicAdminStaffResponseDTO YOKResult { get; set; }
        public virtual List<GraduationDetailResponseDTO> EgitimBilgisiResult { get; set; }
        public virtual CKYSStudent CKYSStudentResult { get; set; }
    }
}

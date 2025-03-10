using Shared.BaseModels;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class UserRoleDTO : UserRoleBase
    {
        public virtual IList<UserRoleFacultyResponseDTO> UserRoleFaculties { get; set; }
        public virtual IList<UserRoleHospitalResponseDTO> UserRoleHospitals { get; set; }
        public virtual IList<UserRoleProgramResponseDTO> UserRolePrograms { get; set; }
        public virtual IList<UserRoleProvinceResponseDTO> UserRoleProvinces { get; set; }
        public virtual IList<UserRoleUniversityResponseDTO> UserRoleUniversities { get; set; }
    }
}
using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class UserRoleResponseDTO : UserRoleBase
    {
        public long? Id { get; set; }
        public UserAccountDetailInfoResponseDTO User { get; set; }
        public RoleResponseDTO Role { get; set; }
        public virtual IList<UserRoleFacultyResponseDTO> UserRoleFaculties { get; set; }
        public virtual IList<UserRoleHospitalResponseDTO> UserRoleHospitals { get; set; }
        public virtual IList<UserRoleProgramResponseDTO> UserRolePrograms { get; set; }
        public virtual IList<UserRoleProvinceResponseDTO> UserRoleProvinces { get; set; }
        public virtual IList<UserRoleUniversityResponseDTO> UserRoleUniversities { get; set; }
    }
}

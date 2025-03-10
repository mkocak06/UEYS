using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class UserRoleUniversityResponseDTO : UserRoleUniversityBase
    {
        public UserRoleResponseDTO UserRole { get; set; }
        public UniversityResponseDTO University { get; set; }
    }
}

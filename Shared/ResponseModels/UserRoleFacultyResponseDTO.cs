using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class UserRoleFacultyResponseDTO : UserRoleFacultyBase
    {
        public UserRoleResponseDTO UserRole { get; set; }
        public FacultyResponseDTO Faculty { get; set; }
        public ExpertiseBranchResponseDTO ExpertiseBranch { get; set; }
    }
}

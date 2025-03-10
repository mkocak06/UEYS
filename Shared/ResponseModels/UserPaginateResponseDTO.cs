using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class UserPaginateResponseDTO
    {
        public long Id { get; set; }
        public string IdentityNo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<string> RoleCodes { get; set; }
		public List<string> Roles { get; set; }
        public List<string> HospitalZones { get; set; }
        public List<string> FacultyZones { get; set; }
        public List<string> UniversityZones { get; set; }
        public List<string> ProgramZones { get; set; }
        public List<string> ProvinceZones { get; set; }
        public string EducatorZone { get; set; }
        public string StudentZone { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Authorization
{
    public class ZoneModelDTO
    {
        public RoleCategoryType RoleCategory { get; set; }
        public List<FacultyResponseDTO> Faculties { get; set; }
        public List<HospitalResponseDTO> Hospitals { get; set; }
        public List<ProgramResponseDTO> Programs { get; set; }
        public List<ProvinceResponseDTO> Provinces { get; set; }
        public List<UniversityResponseDTO> Universities { get; set; }
    }
}

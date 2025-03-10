using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class FacultyResponseDTO : FacultyBase
    {
        public FacultyResponseDTO()
        {
            University = new UniversityResponseDTO();
        }
        public virtual UniversityResponseDTO University { get; set; }
        public virtual ProfessionResponseDTO Profession { get; set; }
        public virtual ICollection<ProgramResponseDTO> Programs { get; set; }
        public virtual ICollection<AffiliationResponseDTO> Affiliations { get; set; }
        public virtual ICollection<HospitalResponseDTO> Hospitals { get; set; }
    }
}

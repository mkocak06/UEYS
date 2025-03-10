using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class UniversityResponseDTO : UniversityBase
    {
        public UniversityResponseDTO()
        {
            //Affiliations = new List<AffiliationResponseDTO>();
            Programs = new List<ProgramResponseDTO>();
            Faculties = new List<FacultyResponseDTO>();
        }
        public long Id { get; set; }
        public virtual ProvinceResponseDTO Province { get; set; }
        public virtual InstitutionResponseDTO Institution { get; set; }

        //public virtual ICollection<AffiliationResponseDTO> Affiliations { get; set; }
        public virtual ICollection<ProgramResponseDTO> Programs { get; set; }
        public virtual List<FacultyResponseDTO> Faculties { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class UniversityDTO : UniversityBase
    {
        public virtual ICollection<AffiliationDTO> Affiliations { get; set; }
        public virtual ICollection<ProgramDTO> Programs { get; set; }
        public virtual ICollection<FacultyDTO> Faculties { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.RequestModels
{
    public class HospitalDTO : HospitalBase
    {
        public ICollection<ProgramDTO> Programs { get; set; }
        public ICollection<AffiliationDTO> Affiliations { get; set; }
    }
}

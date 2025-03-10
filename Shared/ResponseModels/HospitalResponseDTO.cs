using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class HospitalResponseDTO : HospitalBase
    {
        public long Id { get; set; }
        public virtual ProvinceResponseDTO Province { get; set; }
        public virtual InstitutionResponseDTO Institution { get; set; }
        public virtual UserAccountDetailInfoResponseDTO Manager { get; set; }
        public virtual FacultyResponseDTO Faculty { get; set; }

        public ICollection<ProgramResponseDTO> Programs { get; set; }
        public ICollection<AffiliationResponseDTO> Affiliations { get; set; }
    }
}

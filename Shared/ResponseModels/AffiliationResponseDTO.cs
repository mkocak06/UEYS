using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class AffiliationResponseDTO : AffiliationBase
    {
        public long Id { get; set; }
        public virtual FacultyResponseDTO Faculty { get; set; }
        public virtual HospitalResponseDTO Hospital { get; set; }

        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
    }
}

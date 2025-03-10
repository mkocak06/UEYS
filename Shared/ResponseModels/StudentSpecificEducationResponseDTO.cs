using Shared.BaseModels;
using Shared.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class StudentSpecificEducationResponseDTO : StudentSpecificEducationBase
    {
        public long Id { get; set; }
        public virtual SpecificEducationResponseDTO SpecificEducation { get; set; }
        public virtual SpecificEducationPlaceResponseDTO SpecificEducationPlace { get; set; }
        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}

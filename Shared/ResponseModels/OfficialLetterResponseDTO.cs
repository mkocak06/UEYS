using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class OfficialLetterResponseDTO : OfficialLetterBase
    {
        public virtual ThesisResponseDTO Thesis { get; set; }
        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
    }
}

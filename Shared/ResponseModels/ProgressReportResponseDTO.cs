using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class ProgressReportResponseDTO : ProgressReportBase
    {
        
        public virtual ThesisResponseDTO Thesis { get; set; }
        public virtual EducatorResponseDTO Educator { get; set; }
        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
    }
}

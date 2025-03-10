using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class EducationTrackingResponseDTO : EducationTrackingBase
    {
        public StudentResponseDTO Student { get; set; }
        public EducatorResponseDTO ProcessOwner { get; set; }
        public ProgramResponseDTO FormerProgram { get; set; }
        public ProgramResponseDTO Program { get; set; }
        public EducationTrackingResponseDTO RelatedEducationTracking { get; set; }
        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
    }
}

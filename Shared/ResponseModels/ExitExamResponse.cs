using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class ExitExamResponseDTO : ExitExamBase
    {
        public virtual HospitalResponseDTO Hospital { get; set; }
        public virtual StudentResponseDTO Student { get; set; }
        public virtual EducationTrackingResponseDTO EducationTracking { get; set; }
        public UserResponseDTO Secretary { get; set; }

        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public virtual List<JuryResponseDTO> Juries { get; set; }
    }
}

using Shared.BaseModels;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class ExitExamDTO : ExitExamBase
    {
        public virtual EducationTrackingDTO EducationTracking { get; set; }
        public virtual ICollection<JuryDTO> Juries { get; set; }
    }
}

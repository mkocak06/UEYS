using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class StudyResponseDTO : StudyBase
    {
        public long Id { get; set; }

        public virtual ICollection<ScientificStudyResponseDTO> ScientificStudies { get; set; }

    }
}

using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class ScientificStudyResponseDTO : ScientificStudyBase
    {
        public long? Id { get; set; }

        public virtual StudentResponseDTO Student { get; set; }
        public virtual StudyResponseDTO Study { get; set; }
        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
    }
}

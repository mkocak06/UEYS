using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class StudentRotationResponseDTO : StudentRotationBase
    {
        public virtual StudentResponseDTO Student { get; set; }
        public virtual RotationResponseDTO Rotation { get; set; }
        public virtual ProgramResponseDTO Program { get; set; }
        public virtual EducatorResponseDTO Educator { get; set; }
        public virtual ICollection<DocumentResponseDTO> Documents { get; set; }
        public virtual ICollection<StudentRotationPerfectionResponseDTO> StudentRotationPerfections { get; set; }
    }
}

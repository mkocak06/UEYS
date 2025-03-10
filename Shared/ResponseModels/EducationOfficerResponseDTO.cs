using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class EducationOfficerResponseDTO : EducationOfficerBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public ProgramResponseDTO Program { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
    }
}

using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class EducationOfficerDTO : EducationOfficerBase
    {
        public virtual List<DocumentDTO> Documents { get; set; }
    }
}

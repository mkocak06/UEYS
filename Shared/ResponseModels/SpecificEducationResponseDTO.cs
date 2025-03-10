using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class SpecificEducationResponseDTO : SpecificEducationBase
    {
        public long Id { get; set; }
        public virtual ICollection<StudentSpecificEducationResponseDTO> StudentsSpecificEducations { get; set; }
    }
}

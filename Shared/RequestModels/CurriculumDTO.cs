using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestModels
{
    public class CurriculumDTO : CurriculumBase
    {
        public virtual ICollection<CurriculumRotationDTO> CurriculumRotations { get; set; }
        public virtual ICollection<CurriculumPerfectionDTO> CurriculumPerfections { get; set; }
        public virtual ICollection<StudentDTO> Students { get; set; }
    }
}

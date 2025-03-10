using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class StandardResponseDTO : StandardBase
    {
        public virtual long Id { get;  set; }
        public virtual StandardCategoryResponseDTO StandardCategory { get; set; }
        public virtual CurriculumResponseDTO Curriculum { get; set; }
        public virtual ICollection<FormStandardResponseDTO> FormStandards { get; set; }
    }
}

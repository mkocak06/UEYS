using Shared.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class StandardCategoryResponseDTO : StandardCategoryBase
    {
        public virtual long Id { get; set; }
        public virtual ICollection<StandardResponseDTO> Standards { get; set; }
    }
}

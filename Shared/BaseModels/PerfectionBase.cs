using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class PerfectionBase
    {
        public PerfectionType PerfectionType { get; set; }
        public bool? IsRequired { get; set; }
        public string SpecialProvision { get; set; }
        public virtual long? CurriculumRotationId { get; set; }
        public virtual long? CurriculumId { get; set; }
        public bool IsDeleted { get; set; }
    }
}

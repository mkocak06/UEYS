using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Property : BaseEntity
    {
        public string Name { get; set; }
        public PropertyType PropertyType { get; set; }
        public PerfectionType? PerfectionType { get; set; }

        public virtual ICollection<PerfectionProperty> PerfectionProperties { get; set; }
    }
}

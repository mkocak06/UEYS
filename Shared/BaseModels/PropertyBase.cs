using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class PropertyBase
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public PropertyType? PropertyType { get; set; }
        public PerfectionType? PerfectionType { get; set; } 
    }
}

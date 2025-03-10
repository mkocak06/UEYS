using Shared.BaseModels;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestModels
{
    public class PerfectionDTO : PerfectionBase
    {
        public PropertyDTO PName { get; set; }
        public PropertyDTO Group { get; set; }
        public PropertyDTO Seniorty { get; set; }
        public List<PropertyDTO> LevelList { get; set; }
        public List<PropertyDTO> MethodList { get; set; }
    }
}

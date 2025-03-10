using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.FilterModels.Base
{
    public class FilterDTO
    {
        public IEnumerable<Sort> Sort { get; set; }
        public Filter Filter { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string Key { get; set; }
    }
}

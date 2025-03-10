using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.FilterModels.Base
{
    public class Sort
    {
        public string Field { get; set; }
        public SortType Dir { get; set; }
    }
}

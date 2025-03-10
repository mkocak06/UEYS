using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class StandardBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long? StandardCategoryId { get; set; }
        public long? CurriculumId { get; set; }
        public string Code { get; set; }
    }
}

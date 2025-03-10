using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.University
{
    public class UniversityBreadcrumbDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ProgramId { get; set; }
    }
}

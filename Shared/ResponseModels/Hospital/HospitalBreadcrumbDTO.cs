using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Hospital
{
    public class HospitalBreadcrumbDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ProgramId { get; set; }
    }
}

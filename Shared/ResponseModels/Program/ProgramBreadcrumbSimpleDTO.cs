using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Program
{
    public class ProgramBreadcrumbSimpleDTO
    {
        public long Id { get; set; }
        public string ExpertiseBranchName { get; set; }
        public long? ExpertiseBranchId { get; set; }
    }
}

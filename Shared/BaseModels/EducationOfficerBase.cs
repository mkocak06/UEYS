using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class EducationOfficerBase
    {
        public long? Id { get; set; }
        public long? EducatorId { get; set; }
        public long? ProgramId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DocumentOrder { get; set; }

    }
}

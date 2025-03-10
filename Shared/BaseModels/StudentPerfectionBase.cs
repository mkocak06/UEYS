using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class StudentPerfectionBase
    {
        public DateTime? ProcessDate { get; set; }
        public int? Experience { get; set; }
        public bool? IsSuccessful { get; set; }

        public long? StudentId { get; set; }
        public long? PerfectionId { get; set; }
        public long? ProgramId { get; set; }
        public long? EducatorId { get; set; }
    }
}

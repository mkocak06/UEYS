using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BaseModels
{
    public class CurriculumBase
    {
        public bool IsActive { get; set; } = true;
        public string Version { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string DecisionNo { get; set; }
        public int? Duration { get; set; }

        public bool? IsDeleted { get; set; }

        public long? ExpertiseBranchId { get; set; }
    }
}

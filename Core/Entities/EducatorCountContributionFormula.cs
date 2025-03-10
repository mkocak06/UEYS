using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EducatorCountContributionFormula : BaseEntity
    {
        public int? MinEducatorCount { get; set; }
        public int? MaxEducatorCount { get; set; }
        public double? Coefficient { get; set; }
        public double? BaseScore { get; set; }
        public bool? IsExpert { get; set; }

    }
}

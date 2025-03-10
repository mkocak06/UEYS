using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ExitExamRulesModel
    {
        public bool IsThesisSuccess { get; set; }
        public bool AreRotationsCompleted { get; set; }
        public bool AreOpinionFormsCompleted { get; set; }
        public DateTime? EstimatedEndDate{ get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum ProcessType
    {
        [Description("Start")]
        Start = 1,
        [Description("Time Increasing")]
        TimeIncreasing = 2,
        [Description("Time Decreasing")]
        TimeDecreasing = 3,
        [Description("Finish")]
        Finish = 4,
        [Description("Estimated Finish of Education")]
        EstimatedFinish = 5,
        [Description("Transfer")]
        Transfer = 6,
        [Description("Information")]
        Information = 7,
        [Description("Assignment")]
        Assignment = 8,
        [Description("Graduate")]
        Graduate = 9,
    }
}

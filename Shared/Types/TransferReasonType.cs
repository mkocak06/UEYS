using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum TransferReasonType
    {
        [Description("Excused")]
        Excused,
        [Description("Spouse Related")]
        SpouseRelated,
        [Description("Health Related")]
        HealthRelated,
        [Description("Without Excuse")]
        WithoutExcuse,
        [Description("Branch Change")]
        BranchChange
    }
}

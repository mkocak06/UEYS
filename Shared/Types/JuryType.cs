using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum JuryType
    {
        [Description("Core")]
        Core,
        [Description("Alternate")]
        Alternate
    }
}

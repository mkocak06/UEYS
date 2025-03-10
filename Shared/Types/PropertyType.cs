using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Shared.Types
{
    public enum PropertyType
    {
        
        [Description("Grup")]
        PerfectionGroup,
        [Description("Name")]
        PerfectionName,
        [Description("Level")]
        PerfectionLevel,
        [Description("Method")]
        PerfectionMethod,
        [Description("Seniorty")]
        PerfectionSeniorty
    }
}

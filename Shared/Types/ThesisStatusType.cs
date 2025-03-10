using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum ThesisStatusType
    {
        //Attention: It is important that the types are in this order.//
        
        [Description("In Progress")]
        Continues,
        [Description("Successful.")]
        Successful,
        [Description("Unsuccessful.")]
        Unsuccessful

    }
}

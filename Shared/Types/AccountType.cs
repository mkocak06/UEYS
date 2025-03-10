using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum AccountType
    {
        [Description("Süper Admin")]
        SuperAdmin,
        [Description("Admin")]
        Admin,
        [Description("ReadOnly")]
        ReadOnly

    }
}

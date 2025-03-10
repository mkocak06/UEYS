using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum CommitteeOpinionType
    {
        [Description("Uygundur")]
        Suitable =1,
        [Description("Uygun Değildir")]
        NotSuitable=2,
        [Description("Tuk Görüşüne Sunulsun")]
        ReserveToTuk =3


    }
}

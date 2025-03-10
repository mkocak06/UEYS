using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum ForeignType
    {
        [Description("Yabancı Uyruklu")]
        Foreign,
        [Description("Mavi Kart")]
        BlueCard,
        [Description("Türk Soylu (2527 Sayılı Kanun)")]
        TurkishFamily
    }
}

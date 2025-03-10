using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum VisitStatusType
    {
        [Description("Talep İletildi")]
        RequestCreated,
        [Description("Komisyon Üyeleri Atandı")]
        CommitteeAppointed,
        [Description("TUK'a sevk edildi")]
        CommitteeEvaluated,
        [Description("TUK Değerlendirmesi Sonuçlandı")]
        TUKEvaluated,
    }
}

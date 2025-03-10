using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum ThesisSubjectType_2
    {
        [Description("Prospektif")]
        Prospektif,
        [Description("Retrospektif")]
        Retrospektif,
        [Description("Animal Studies")]
        Animal,
        [Description("Invitro (Inanimate) Studies")]
        Invitro,
        [Description("Sectional")]
        Sectional,
        [Description("Insilico (Simulation) Studies")]
        Insilico,
    }
}
    
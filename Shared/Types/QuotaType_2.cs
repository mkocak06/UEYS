using System.ComponentModel;

namespace Shared.Types
{
    public enum QuotaType_2
    {
        //Ministry Of Health
        [Description("EAH")]
        EAH,
        [Description("SBA")]
        SBA,

        //Uni
        [Description("State University")]
        University_State,
        [Description("Private University")]
        University_Private,

        //KKTC
        [Description("KKTC Full Time")]
        KKTCFullTime,
        [Description("KKTC Half Time")]
        KKTCHalfTime,

        //Ministry of Interior
        [Description("JGK")]
        JGK,

        //MSB
        [Description("KKK")]
        KKK,
        [Description("HKK")]
        HKK,
        [Description("DKK")]
        DKK,

        //TDMM
        [Description("Vet")]
        Vet,
        [Description("Chemist")]
        Chemist,
        [Description("Pharmacist")]
        Pharmacist
    }
}

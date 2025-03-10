using System.ComponentModel;

namespace Shared.Types
{
    public enum QuotaType
    {
        [Description("YBU(Foreign National)")]
        YBU,
        [Description("ADL(Forensic Medicine Institute)")]
        ADL,
        [Description("MAP(Guest Military Personnel)")]
        MAP,
        //Ministry Of Health
        [Description("SB(Ministry Of Health)")]
        SB,

        //Uni
        [Description("University(State)")]
        Uni_State,
        [Description("University(Private)")]
        Uni_Private,
        [Description("SBA(On behalf of the Ministry of Health)")]
        SBA,
        //KKTC
        [Description("KKTC Full Time")]
        KKTCFullTime,
        [Description("KKTC Half Time")]
        KKTCHalfTime,

        //Ministry of Interior
        [Description("Ministry Of Interior-JGK(Gendarmerie General Command)")]
        JGK,

        //MSB
        [Description("MSB(National Defense Department)-KKK(Land Forces Command)")]
        KKK,
        [Description("MSB(National Defense Department)-HKK(Air Force Command)")]
        HKK,
        [Description("MSB(National Defense Department)-DKK(Naval Forces Command)")]
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

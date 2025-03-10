using System.Collections.Generic;

namespace Shared.Models
{
    public class CKYS_DoctorListModel
    {
        public List<CKYSDoctor> DrList { get; set; }
    }

    public class CKYSDoctor
    {
        public string WorkPlaceName { get; set; }
        public string WorkSituation { get; set; }
        public long? StaffTitleId { get; set; }
        public string StaffTitleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<DoctorExpertiseBranch> DoctorExpertiseBranches { get; set; }
    }

    public class CKYSStudent
    {
        public string GraduatedSchool { get; set; }
        public string GraduatedDate { get; set; }
        public string MedicineRegistrationDate { get; set; }
        public string MedicineRegistrationNo { get; set; }
        public long? StaffTitleId { get; set; }
        public string StaffTitleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<DoctorExpertiseBranch> DoctorExpertiseBranches { get; set; }

    }

    public class CKYSTest
    {
        public string mernis_noField;

        public string psnField;

        public string adField;

        public string soyadField;

        public string v_aktif_birim_kodField;

        public string v_aktif_birim_adField;

        public string v_gecici_gorev_birim_kodField;

        public string v_gecici_gorev_birim_adField;

        public string v_calisma_durumuField;

        public string v_aktif_unvan_kodField;

        public string v_aktif_unvan_adField;

        public string v_ihtisas_unvan_kodField;

        public string v_ihtisas_unvan_adField;

        public string v_ogrencimiField;

        public string v_basasistan_olma_trhField;

        public string dhk_mezun_okulField;

        public string dhk_tescil_noField;

        public string dhk_tescil_trhField;

        public string dhk_dip_noField;

        public string dhk_meslek_unvaniField;

        public string tip_mezun_okulField;

        public string tip_tescil_noField;

        public string tip_tescil_trhField;

        public string tip_dip_noField;

        public string tip_meslek_unvaniField;

        public string uzm1_tescil_noField;

        public string uzm1_tescil_trhField;

        public string uzm1_brans_koduField;

        public string uzm1_brans_adiField;

        public string uzm1_mezun_okulField;

        public string uzm1_meslek_unvaniField;

        public string uzm2_tescil_noField;

        public string uzm2_tescil_trhField;

        public string uzm2_brans_koduField;

        public string uzm2_brans_adiField;

        public string uzm2_mezun_okulField;

        public string uzm2_meslek_unvaniField;

        public string uzm3_tescil_noField;

        public string uzm3_tescil_trhField;

        public string uzm3_brans_koduField;

        public string uzm3_brans_adiField;

        public string uzm3_mezun_okulField;

        public string uzm3_meslek_unvaniField;

        public string uzm4_tescil_noField;

        public string uzm4_tescil_trhField;

        public string uzm4_brans_koduField;

        public string uzm4_brans_adiField;

        public string uzm4_mezun_okulField;

        public string uzm4_meslek_unvaniField;

        public string uzm5_tescil_noField;

        public string uzm5_tescil_trhField;

        public string uzm5_brans_koduField;

        public string uzm5_brans_adiField;

        public string uzm5_mezun_okulField;

        public string uzm5_meslek_unvaniField;

        public string tescil_trhField;

        public string guncelleme_trhField;

        public string dhk_iptal_trhField;

        public string dhk_iptal_sebepField;

        public string dhk_iptal_ackField;

        public string tip_iptal_trhField;

        public string tip_iptal_sebepField;

        public string tip_iptal_ackField;

        public string uzm1_iptal_trhField;

        public string uzm1_iptal_sebepField;

        public string uzm1_iptal_ackField;

        public string uzm2_iptal_trhField;

        public string uzm2_iptal_sebepField;

        public string uzm2_iptal_ackField;

        public string uzm3_iptal_trhField;

        public string uzm3_iptal_sebepField;

        public string uzm3_iptal_ackField;

        public string uzm4_iptal_trhField;

        public string uzm4_iptal_sebepField;

        public string uzm4_iptal_ackField;

        public string uzm5_iptal_trhField;

        public string uzm5_iptal_sebepField;

        public string uzm5_iptal_ackField;

        public string dhk_mez_trhField;

        public string tip_mez_trhField;

        public string uzm1_mez_trhField;

        public string uzm2_mez_trhField;

        public string uzm3_mez_trhField;

        public string uzm4_mez_trhField;

        public string uzm5_mez_trhField;

        public string kayit_seqField;

        public string sorgu_trhField;

        public string tip_mezun_fakulteField;

        public string dhk_mezun_fakulteField;

        public string emailField;

        public string cep_noField;
    }

    public class DoctorExpertiseBranch
    {
        public string RegistrationDate { get; set; }
        public string RegistrationNo { get; set; }
        public string RegistrationBranchName { get; set; }
        public string RegistrationGraduationSchool{ get; set; }
        public string ExpertiseBranchName { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public bool? IsPrincipal { get; set; }
        public List<long> SubBrIds { get; set; }
    }
}


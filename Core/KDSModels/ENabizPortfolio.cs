using System;

namespace Core.KDSModels
{
    public class ENabizPortfolio
    {
        public long id { get; set; }
        public string yilay { get; set; }
        public string hekim_kimlik_numarasi { get; set; }
        public string hizmet_sunucu { get; set; }
        public string kurum_adi { get; set; }
        public string klinik_kodu { get; set; }
        public string klinik_adi { get; set; }
        public string il_kodu { get; set; }
        public string il_adi { get; set; }
        public int? ilce_kodu { get; set; }
        public int? islem_sayisi { get; set; }
        public int? muayene_sayisi { get; set; }
        public int? ameliyat_ve_girisimler_sayisi { get; set; }
        public int? diger_islemler_sayisi { get; set; }
        public int? dis_islemleri_sayisi { get; set; }
        public int? dogum_islemleri_sayisi { get; set; }
        public int? kan_islemleri_sayisi { get; set; }
        public int? malzeme_sayisi { get; set; }
        public int? tahlil_tetkik_ve_radyoloji_islemleri_sayisi { get; set; }
        public int? yatak_islemleri_sayisi { get; set; }
        public int? a_grubu_ameliyat_sayisi { get; set; }
        public int? b_grubu_ameliyat_sayisi { get; set; }
        public int? c_grubu_ameliyat_sayisi { get; set; }
        public int? d_grubu_ameliyat_sayisi { get; set; }
        public int? e_grubu_ameliyat_sayisi { get; set; }
        public int? recete_sayisi { get; set; }
        public int? ilac_sayisi { get; set; }
        public DateTime? guncelleme_zamani { get; set; }
    }
}

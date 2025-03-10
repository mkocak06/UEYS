using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EkipModels
{
    public class PersonelHareketi
    {
        public long? mv_id { get; set; }
        public DateTime? mv_refresh_time { get; set; }
        public short? turkod { get; set; }
        public string turad { get; set; }
        public long? sira { get; set; }
        public long? employee_id { get; set; }
        public long? psn { get; set; }
        public string tckn { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string teskilat { get; set; }
        public DateTime? grv_bsl_trh { get; set; }
        public DateTime? grv_ayr_trh { get; set; }
        public DateTime? baslama_tarihi { get; set; }
        public DateTime? bitis_tarihi { get; set; }
        public string unvan_ad { get; set; }
        public string birim_ad { get; set; }
        public string brans_ad { get; set; }
        public string dis_kurum_birim_ad { get; set; }
        public string dis_unvan_ad { get; set; }
        public string dis_brans_ad { get; set; }
        public string dis_yandal_ad { get; set; }
        public long? birim_kod { get; set; }
        public string birim_kod2 { get; set; }
        public string unvan_kod { get; set; }
        public string unvan_kod2 { get; set; }
        public string brans_kod { get; set; }
        public string brans_kod2 { get; set; }
        public string dis_kurum_birim_kod { get; set; }
        public string dis_unvan_kod { get; set; }
        public string dis_brans_kod { get; set; }
        public string dis_yandal_kod { get; set; }
        public string kod { get; set; }
        public string kod2 { get; set; }
        public string kod_ack { get; set; }
        public string kod_ack2 { get; set; }
        public string dayanak_ack { get; set; }
        public string dayanak_ack2 { get; set; }
        public string per_hrk_seq { get; set; }
        public string per_hrk_seq2 { get; set; }
        public string hrk_onay_seq { get; set; }
        public string hrk_onay_seq2 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class TaahhutModel
    {
        public string Id { get; set; }
        public string tckn { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string anadal { get; set; }
        public string yandal { get; set; }
        public string akademikUnvan { get; set; }
        public string kadroUnvan { get; set; }
        public string ustKurum { get; set; }
        public string uzmanlikOgrencisiMi { get; set; }
        public string dis_kurum_kod { get; set; }
        public string dis_kurum_ad { get; set; }
        public string dis_birim_kod { get; set; }
        public string dis_birim_ad { get; set; }
        public string dis_unvan_kod { get; set; }
        public string dis_unvan_ad { get; set; }
        public string dis_brans_kod { get; set; }
        public string dis_brans_ad { get; set; }
        public string birim_kod { get; set; }
        public string birim_ad { get; set; }
        public string unvan_kod { get; set; }
        public string unvan_ad { get; set; }
        public string brans_kod { get; set; }
        public string brans_ad { get; set; }
        public string yandal_kod { get; set; }
        public string yandal_ad { get; set; }
        public string protokol_baslama_tarihi { get; set; }
        public string protokol_bitis_tarihi { get; set; }
    }
}

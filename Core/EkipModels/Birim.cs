using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EkipModels
{
    public class Birim
    {
        public long mv_id { get; set; }
        public long? birim_kod { get; set; }
        public string birim_ad { get; set; }
        public long? ust_birim_kod { get; set; }
        public string adres1 { get; set; }
        public string telefon1 { get; set; }
        public string e_posta { get; set; }
        public string il_ad { get; set; }
        public string ilce_ad { get; set; }
        public string faal_drm_ack { get; set; }
    }
}

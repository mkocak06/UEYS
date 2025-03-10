using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Ekip
{
    public class PersonelResponseDTO
    {
        public long mv_id { get; set; }
        public long id { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string tckn { get; set; }
        public DateTime? dogum_tarihi { get; set; }
        public long? aktif_birim_kod { get; set; }
        public string aktif_birim_ad { get; set; }
        public string aktif_unvan_kod { get; set; }
        public string aktif_unvan_ad { get; set; }
        public string aktif_brans_kod { get; set; }
        public string aktif_brans_ad { get; set; }
        public int? aktif { get; set; }
        public string calisma_durumu { get; set; }
    }
}

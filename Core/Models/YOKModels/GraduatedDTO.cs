using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.YOKModels
{
    public class GraduatedDTO
    {
        [JsonProperty(PropertyName = "TckNo")]
        public long? TckNo { get; set; }

        [JsonProperty(PropertyName = "Ad")]
        public string Ad { get; set; }

        [JsonProperty(PropertyName = "Soyad")]
        public string Soyad { get; set; }

        [JsonProperty(PropertyName = "AnneAd")]
        public string AnneAd { get; set; }

        [JsonProperty(PropertyName = "BabaAd")]
        public string BabaAd { get; set; }

        [JsonProperty(PropertyName = "DogumTarih")]
        public DateTime DogumTarih { get; set; }

        [JsonProperty(PropertyName = "MezuniyetTarih")]
        public DateTime MezuniyetTarih { get; set; }

        [JsonProperty(PropertyName = "DiplomaNot")]
        public double DiplomaNot { get; set; }

        [JsonProperty(PropertyName = "DiplomaNotSistemi")]
        public int? DiplomaNotSistemi { get; set; }

        [JsonProperty(PropertyName = "DiplomaNo")]
        public string DiplomaNo { get; set; }

        [JsonProperty(PropertyName = "UniversiteAd")]
        public string UniversiteAd { get; set; }

        [JsonProperty(PropertyName = "FakMyoYoEns")]
        public string FakMyoYoEns { get; set; }

        [JsonProperty(PropertyName = "ProgramAd")]
        public string ProgramAd { get; set; }

        [JsonProperty(PropertyName = "UniversiteId")]
        public long? UniversiteId { get; set; }

        [JsonProperty(PropertyName = "BirimId")]
        public long? BirimId { get; set; }

        [JsonProperty(PropertyName = "Durum")]
        public string Durum { get; set; }
    }
}

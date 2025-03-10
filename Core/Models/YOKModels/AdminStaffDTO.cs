using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.YOKModels
{
    public class AdminStaffDTO
    {
        [JsonProperty(PropertyName = "TckNo")]
        public long? TckNo { get; set; }

        [JsonProperty(PropertyName = "Ad")]
        public string Ad { get; set; }

        [JsonProperty(PropertyName = "Soyad")]
        public string Soyad { get; set; }

        [JsonProperty(PropertyName = "BirimId")]
        public long? BirimId { get; set; }

        [JsonProperty(PropertyName = "BirimAd")]
        public string BirimAd { get; set; }


        [JsonProperty(PropertyName = "KadroId")]
        public long? KadroId { get; set; }

        [JsonProperty(PropertyName = "KadroAd")]
        public string KadroAd { get; set; }


        [JsonProperty(PropertyName = "GorevBirimId")]
        public long? GorevBirimId { get; set; }

        [JsonProperty(PropertyName = "GorevBirimAd")]
        public string GorevBirimAd { get; set; }

        [JsonProperty(PropertyName = "GorevBirimTur")]
        public long? GorevBirimTur { get; set; }


        [JsonProperty(PropertyName = "KadroBirimId")]
        public long? KadroBirimId { get; set; }

        [JsonProperty(PropertyName = "KadroBirimAd")]
        public string KadroBirimAd { get; set; }

        [JsonProperty(PropertyName = "KadroBirimTur")]
        public long? KadroBirimTur { get; set; }
    }
}

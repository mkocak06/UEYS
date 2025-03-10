using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.YOKModels
{
    public class AcademicianExpertiseDTO
    {
        [JsonProperty(PropertyName = "TckNo")]
        public long? TckNo { get; set; }

        [JsonProperty(PropertyName = "Dus")]
        public string Dus { get; set; }

        [JsonProperty(PropertyName = "Eus")]
        public string Eus { get; set; }

        [JsonProperty(PropertyName = "Tus")]
        public string Tus { get; set; }

        [JsonProperty(PropertyName = "Yandal1")]
        public string Yandal1 { get; set; }

        [JsonProperty(PropertyName = "Yandal2")]
        public string Yandal2 { get; set; }
    }
}

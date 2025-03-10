using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.YOKModels
{
    public class AcademicianAssignmentDTO
    {
        [JsonProperty(PropertyName = "TckNo")]
        public long TckNo { get; set; }

        [JsonProperty(PropertyName = "GorevKurumId")]
        public long? GorevKurumId { get; set; }

        [JsonProperty(PropertyName = "GorevBaslangicTarihi")]
        public string GorevBaslangicTarihi { get; set; }

        [JsonProperty(PropertyName = "GorevBitisTarihi")]
        public string GorevBitisTarihi { get; set; }
    }
}

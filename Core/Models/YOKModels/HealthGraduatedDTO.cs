using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.YOKModels
{
    public class HealthGraduatedDTO
    {
        [JsonProperty(PropertyName = "BirimId")]
        public long? BirimId { get; set; }

        [JsonProperty(PropertyName = "TckNo")]
        public long? TckNo { get; set; }
    }
}

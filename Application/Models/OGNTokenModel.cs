using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class OGNTokenModel
    {
        [JsonProperty("id_token")]
        public string IdToken { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}

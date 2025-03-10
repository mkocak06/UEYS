using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class OGNUserInfoModel
    {
        [JsonProperty("sub")]
        public string IdentityNumber { get; set; }
        [JsonProperty("name")]
        public string FullName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("amr")]
        public string AuthenticationMethod { get; set; }
    }
}

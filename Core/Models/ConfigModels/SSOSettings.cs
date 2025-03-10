using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.ConfigModels
{
    public class SSOSettings
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string LogoutUri { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }
        public string OpenIdServer { get; set; }
    }
}

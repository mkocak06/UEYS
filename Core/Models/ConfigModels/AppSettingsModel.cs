using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.ConfigModels
{
    public class AppSettingsModel
    {
        public CKYSSettings CKYS { get; set; }
        public S3Settings S3 { get; set; }
        public KPSSettings KPS { get; set; }
        public SSOSettings SSO { get; set; }
        public SMSSettings SMS { get; set; }
        public YOKSettings YOK { get; set; }
        public EnvironmentSettings Environment { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
        public TokenOptions TokenOptions { get; set; }
    }
}

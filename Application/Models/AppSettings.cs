using Core.Models;
using Core.Models.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class AppSettings
    {
        public IDictionary<string, string> ConnectionStrings { get; set; }
        public string this[string key] { get => ConnectionStrings[key]; }
        public IDictionary<string, string> TokenOptions { get; set; }
        public IDictionary<string, string> EmailConfiguration { get; set; }

        public IDictionary<string, string> SSO { get; set; }
        public IDictionary<string, string> CKYS { get; set; }
        public IDictionary<string, string> KPS { get; set; }
        public IDictionary<string, string> YOK { get; set; }
        public IDictionary<string, string> S3 { get; set; }
        public IDictionary<string, string> SMS { get; set; }
        public IDictionary<string, string> ENVIRONMENT { get; set; }


        public AppSettings() { }
    }
}

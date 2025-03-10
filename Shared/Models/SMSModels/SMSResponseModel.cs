using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.SMSModels
{
    public class SMSResponseModel
    {
        public string RequestKey { get; set; }
        public List<string> StatusDetails { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Message { get; set; }
    }
}

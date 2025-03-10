using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.SMSModels
{
    public class BulkSMSModel
    {
        public string SmsContent { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Captcha
{
    public class CaptchaChiperModel
    {
        public string Key { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}

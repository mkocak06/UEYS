using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum ErrorType
    {
        Success = 0,
        ExceptionError = 1,
        [Description("Email is already taken")]
        EmailExist = 2,
        [Description("Email or password is wrong")]
        EmailNotFound = 3,
        [Description("Captcha expired")]
        CaptchaExpireTime = 4,
        [Description("Captcha is wrong")]
        CaptchaWrong = 5,
    }
}

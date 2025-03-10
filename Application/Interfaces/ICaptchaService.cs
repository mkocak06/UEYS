using Shared.ResponseModels.Captcha;
using Shared.ResponseModels.Wrapper;
using SixLaborsCaptcha.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICaptchaService
    {
        ResponseWrapper<CaptchaResponseDTO> GetCaptcha(ISixLaborsCaptchaModule sixLaborsCaptcha);
        ResponseWrapper<bool> VerifyCaptcha(string chiper, string data);
    }
}

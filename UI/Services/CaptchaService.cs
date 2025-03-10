using Shared.FilterModels.Base;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.ResponseModels.Captcha;

namespace UI.Services
{

    public interface ICaptchaService
    {
        Task<ResponseWrapper<CaptchaResponseDTO>> Get();
        Task<ResponseWrapper<bool>> VerifyCaptcha(string chiper, string data);
    }
    public class CaptchaService : ICaptchaService
    {
        private readonly IHttpService _httpService;

        public CaptchaService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<CaptchaResponseDTO>> Get()
        {
            return await _httpService.Get<ResponseWrapper<CaptchaResponseDTO>>("Captcha/Get");
        }
        public async Task<ResponseWrapper<bool>> VerifyCaptcha(string chiper, string data)
        {
            return await _httpService.Get<ResponseWrapper<bool>>($"Captcha/Verify?chiper={chiper}&data={data}");
        }
    }

}

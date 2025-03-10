using Application.Interfaces;
using Shared.Extensions;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Captcha;
using Shared.ResponseModels.Wrapper;
using SixLaborsCaptcha.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly ICryptoService cryptoService;
        public CaptchaService(ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }
        public ResponseWrapper<CaptchaResponseDTO> GetCaptcha(ISixLaborsCaptchaModule sixLaborsCaptcha)
        {

            ResponseWrapper<CaptchaResponseDTO> response = new();
            try
            {
                string key = Extensions.GetUniqueKey(6, new char[] {'1', '2', '3', '4', '5', '6', '7', '8', '9'});
                var imgText = sixLaborsCaptcha.Generate(key);

                CaptchaChiperModel chiperModel = new()
                {
                    Key = key,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5)
                };
                var data = JsonSerializer.Serialize(chiperModel);
                string chiperData = cryptoService.Encrypt(data, true);

                if (!string.IsNullOrEmpty(chiperData))
                {
                    CaptchaResponseDTO captchaResponse = new()
                    {
                        Captcha = Convert.ToBase64String(imgText),
                        Chiper = chiperData
                    };
                    response.Item = captchaResponse;
                    response.Result = true;
                }
                else
                {
                    response.Result = false;
                    response.Message = "Unable to load captcha";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToString();
                return response;
            }
        }
        public ResponseWrapper<bool> VerifyCaptcha(string chiper, string data)
        {
            string chiperData = cryptoService.Decrypt(chiper, true);
            var desData = JsonSerializer.Deserialize<CaptchaChiperModel>(chiperData);
            if (desData != null)
            {
                if (desData.ExpiryTime < DateTime.UtcNow)
                {
                    return new() { Result = false, Message = Shared.Types.ErrorType.CaptchaExpireTime.GetDescription() };
                }
                else if (!desData.Key.Equals(data))
                {
                    return new() { Result = false, Message = Shared.Types.ErrorType.CaptchaWrong.GetDescription() };
                }
                return new() { Result = true };
            }
            else
            {
                throw new Exception("Data is null.");
            }

        }
    }
}

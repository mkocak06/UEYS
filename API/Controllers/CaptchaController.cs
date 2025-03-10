using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using SixLaborsCaptcha.Core;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly ICaptchaService _captchaService;

        public CaptchaController(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        [HttpGet]
        public IActionResult Get([FromServices] ISixLaborsCaptchaModule sixLaborsCaptcha)
            => Ok(_captchaService.GetCaptcha(sixLaborsCaptcha));

        [HttpGet]
        public IActionResult Verify(string chiper, string data)
            => Ok(_captchaService.VerifyCaptcha(chiper, data));
    }
}

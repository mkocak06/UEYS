using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("ExternalIntegrationPolicy")]
    public class OSYMController : ControllerBase
    {
        private readonly IOSYMService osymService;

        public OSYMController(IOSYMService osymService)
        {
            this.osymService = osymService;
        }

        [HttpGet]
        [HasPermission(PermissionEnum.OSYMGetStudentInformation)]
        public async Task<IActionResult> AktifOgrenciMi(CancellationToken cancellationToken, string sinavTuru, string kimlikNo, DateTime? oncekiSinavTarihi, DateTime? mevcutSinavTarihi) => Ok(await osymService.IsActiveStudent(cancellationToken, sinavTuru, kimlikNo, oncekiSinavTarihi, mevcutSinavTarihi));

        [HttpPost]
        [HasPermission(PermissionEnum.OSYMPostLastExamResults)]
        public async Task<IActionResult> SinavSonuclariniEkle(CancellationToken cancellationToken, List<OSYM_StudentExamResult> ogrenciSinavSonuclari) => Ok(await osymService.AddExamResults(cancellationToken, ogrenciSinavSonuclari));
    }
}

using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Net.Http;
using Shared.FilterModels.Base;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProvinceController : BaseController
    {
        private readonly IProvinceService provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            this.provinceService = provinceService;
        }

        [HttpPost]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await provinceService.GetPaginateList(cancellationToken, filter));

        [HttpGet]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await provinceService.GetListAsync(cancellationToken));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.DashboardGetMap)]
        public async Task<IActionResult> GetProvinceDetailForMap(CancellationToken cancellationToken) => Ok(await provinceService.ProvinceDetailsForMap(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ProvinceGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await provinceService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ProvinceAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ProvinceDTO provinceDTO) => Ok(await provinceService.PostAsync(cancellationToken, provinceDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ProvinceUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ProvinceDTO provinceDTO) => Ok(await provinceService.Put(cancellationToken, id, provinceDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ProvinceDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await provinceService.Delete(cancellationToken, id));
    }
}

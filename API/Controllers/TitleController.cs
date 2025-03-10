using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TitleController : BaseController
    {
        private readonly ITitleService titleService;

        public TitleController(ITitleService titleService)
        {
            this.titleService = titleService;
        }

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.TitleGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await titleService.GetPaginateList(cancellationToken, filter));


        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.TitleGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await titleService.GetListAsync(cancellationToken));

        [HttpGet("{titleType}")]
        //[HasPermission(Shared.Types.PermissionEnum.TitleGetList)]
        public async Task<IActionResult> GetListByTypeAsync(CancellationToken cancellationToken, TitleType titleType) => Ok(await titleService.GetListByTypeAsync(cancellationToken, titleType));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.TitleGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await titleService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.TitleAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] TitleDTO titleDTO) => Ok(await titleService.PostAsync(cancellationToken, titleDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.TitleUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] TitleDTO titleDTO) => Ok(await titleService.Put(cancellationToken, id, titleDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.TitleDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await titleService.Delete(cancellationToken, id));

    }
}

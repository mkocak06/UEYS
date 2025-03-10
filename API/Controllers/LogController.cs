using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading.Tasks;
using System.Threading;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LogController : BaseController
    {
        private readonly ILogService logService;

        public LogController(ILogService logService)
        {
            this.logService = logService;
        }
        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.LogGetListPagination)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, [FromBody] FilterDTO filter) => Ok(await logService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.LogGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await logService.GetByIdAsync(cancellationToken, id));
    }
}

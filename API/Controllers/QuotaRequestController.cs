using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuotaRequestController : ControllerBase
    {
        private readonly IQuotaRequestService quotaRequestService;

        public QuotaRequestController(IQuotaRequestService quotaRequestService)
        {
            this.quotaRequestService = quotaRequestService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.QuotaRequestGetPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await quotaRequestService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.QuotaRequestGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await quotaRequestService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.QuotaRequestAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] QuotaRequestDTO quotaRequestDTO) => Ok(await quotaRequestService.PostAsync(cancellationToken, quotaRequestDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.QuotaRequestUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] QuotaRequestDTO quotaRequestDTO) => Ok(await quotaRequestService.Put(cancellationToken, id, quotaRequestDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.QuotaRequestDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await quotaRequestService.Delete(cancellationToken, id));
    }
}

using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
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
    public class SubQuotaRequestController : ControllerBase
    {
        private readonly ISubQuotaRequestService subQuotaRequestService;

        public SubQuotaRequestController(ISubQuotaRequestService subQuotaRequestService)
        {
            this.subQuotaRequestService = subQuotaRequestService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.SubQuotaRequestGetPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await subQuotaRequestService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.SubQuotaRequestGetPaginateList)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await subQuotaRequestService.ExcelExport(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.SubQuotaRequestGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await subQuotaRequestService.GetByIdAsync(cancellationToken, id));

        [HttpGet("{programId}")]
        [HasPermission(PermissionEnum.SubQuotaRequestGetById)]
        public async Task<IActionResult> GetByProgramId(CancellationToken cancellationToken, long programId) => Ok(await subQuotaRequestService.GetByProgramId(cancellationToken, programId));

        [HttpPost]
        [HasPermission(PermissionEnum.SubQuotaRequestAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] SubQuotaRequestDTO subQuotaRequestDTO) => Ok(await subQuotaRequestService.PostAsync(cancellationToken, subQuotaRequestDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.SubQuotaRequestUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] SubQuotaRequestDTO subQuotaRequestDTO) => Ok(await subQuotaRequestService.Put(cancellationToken, id, subQuotaRequestDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.SubQuotaRequestDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await subQuotaRequestService.Delete(cancellationToken, id));
    }
}

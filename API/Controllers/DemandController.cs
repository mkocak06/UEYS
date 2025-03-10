using Application.Services;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DemandController : BaseController
    {
        private readonly IDemandService _demandService;
        public DemandController(IDemandService demandService)
        {
            _demandService = demandService;
        }
        
        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.DemandGetListPagination)]
        //public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, [FromBody] FilterDTO filter) 
        //    => Ok(await _demandService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.DemandGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) 
        //    => Ok(await _demandService.GetByIdAsync(cancellationToken, id));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.DemandAdd)]
        //public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] DemandDTO demandDto) 
        //    => Ok(await _demandService.PostAsync(cancellationToken, demandDto));

        //[HttpPut("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.DemandUpdate)]
        //public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] DemandDTO demandDto) 
        //    => StatusCode(201, await _demandService.Put(cancellationToken, id, demandDto));

        //[HttpDelete("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.DemandDelete)]
        //public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) 
        //    => Ok(await _demandService.Delete(cancellationToken, id));
    }
}

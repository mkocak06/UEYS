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
    public class StandardController : BaseController
    {
        private readonly IStandardService standardService;

        public StandardController(IStandardService standardService)
        {
           this.standardService = standardService;
        }
        [HttpPost]
        //[HasPermission(PermissionEnum.StandardGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await standardService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{expBranchId}")]
        //[HasPermission(PermissionEnum.StandardGetList)]
        public async Task<IActionResult> GetPaginateListByLatestCurriculumsExpertiseBranch(CancellationToken cancellationToken, long expBranchId) => Ok(await standardService.GetPaginateListByLatestCurriculumsExpertiseBranch(cancellationToken, expBranchId));

        [HttpGet]
        //[HasPermission(PermissionEnum.StandardGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await standardService.GetListAsync(cancellationToken));


        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.StandardGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await standardService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.StandardAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StandardDTO standartDTO) => Ok(await standardService.PostAsync(cancellationToken, standartDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StandardUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StandardDTO standartDTO) => Ok(await standardService.Put(cancellationToken, id, standartDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.StandardDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await standardService.Delete(cancellationToken, id));
    }
}

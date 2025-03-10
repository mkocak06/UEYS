using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;
using Infrastructure.Services;
using Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExitExamController : BaseController
    {
        private readonly IExitExamService exitExamService;
        private readonly IS3Service s3Service;

        public ExitExamController(IExitExamService ExitExamService, IS3Service s3Service)
        {
            this.exitExamService = ExitExamService;
            this.s3Service = s3Service;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ExitExamGetListPagination)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await exitExamService.GetPaginateList(cancellationToken, filter));


        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.ExitExamGetList)]
        //public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await ExitExamService.GetListAsync(cancellationToken));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.ExitExamGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await ExitExamService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ExitExamAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ExitExamDTO ExitExamDTO) => Ok(await exitExamService.PostAsync(cancellationToken, ExitExamDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ExitExamUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ExitExamDTO ExitExamDTO) => Ok(await exitExamService.Put(cancellationToken, id, ExitExamDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ExitExamDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await exitExamService.Delete(cancellationToken, id));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.ExitExamGetListPagination)]
        public async Task<IActionResult> GetExitExamRules(CancellationToken cancellationToken, long studentId) => Ok(await exitExamService.GetExitExamRules(cancellationToken, studentId));
    }
}

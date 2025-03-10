using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly IMobileService mobileService;

        public MobileController(IMobileService mobileService)
        {
            this.mobileService = mobileService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> ProgramGetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await mobileService.ProgramGetPaginateList(cancellationToken, filter));

        [HttpGet]
        [HasPermission(PermissionEnum.ProgramGetById)]
        public async Task<IActionResult> ProgramGetById(CancellationToken cancellationToken, long id) => Ok(await mobileService.ProgramGetById(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> EducatorGetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await mobileService.EducatorGetPaginateList(cancellationToken, filter));

        [HttpGet]
        [HasPermission(PermissionEnum.EducatorGetById)]
        public async Task<IActionResult> EducatorGetById(CancellationToken cancellationToken, long id) => Ok(await mobileService.EducatorGetById(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> StudentGetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await mobileService.StudentGetPaginateList(cancellationToken, filter));

        [HttpGet]
        [HasPermission(PermissionEnum.StudentGetById)]
        public async Task<IActionResult> StudentGetById(CancellationToken cancellationToken, long id) => Ok(await mobileService.StudentGetById(cancellationToken, id));

        [HttpGet]
        //[HasPermission(PermissionEnum.GetUserById)]
        public async Task<IActionResult> UserGetById(CancellationToken cancellationToken) => Ok(await mobileService.UserGetById(cancellationToken));

    }
}

using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationCategoryController : BaseController
    {
        private readonly IAuthorizationCategoryService authorizationCategoryService;

        public AuthorizationCategoryController(IAuthorizationCategoryService authorizationCategoryService)
        {
            this.authorizationCategoryService = authorizationCategoryService;
        }

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationCategoryGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await authorizationCategoryService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationCategoryGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await authorizationCategoryService.GetByIdAsync(cancellationToken, id));
        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationCategoryGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await authorizationCategoryService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationCategoryAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] AuthorizationCategoryDTO authorizationCategoryDTO) => Ok(await authorizationCategoryService.PostAsync(cancellationToken, authorizationCategoryDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationCategoryUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] AuthorizationCategoryDTO authorizationCategoryDTO) => Ok(await authorizationCategoryService.Put(cancellationToken, id, authorizationCategoryDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.AuthorizationCategoryDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await authorizationCategoryService.Delete(cancellationToken, id));

        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.AuthorizationCategoryUpdate)]
        //public async Task<IActionResult> ChangeOrder(CancellationToken cancellationToken, long id, bool isToUp) => Ok(await authorizationCategoryService.ChangeOrder(cancellationToken, id, isToUp));
    }
}

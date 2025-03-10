using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StandardCategoryController : BaseController
    {
        private readonly IStandardCategoryService standardCategoryService;

        public StandardCategoryController(IStandardCategoryService standardCategoryService)
        {
            this.standardCategoryService = standardCategoryService;
        }
        [HttpPost]
        //[HasPermission(PermissionEnum.StandardCategoryGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await standardCategoryService.GetPaginateList(cancellationToken, filter));

        [HttpGet]
        //[HasPermission(PermissionEnum.StandardCategoryGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await standardCategoryService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.StandardCategoryGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await standardCategoryService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.StandardCategoryAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StandardCategoryDTO standardCategoryDTO) => Ok(await standardCategoryService.PostAsync(cancellationToken, standardCategoryDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StandardCategoryUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StandardCategoryDTO standardCategoryDTO) => Ok(await standardCategoryService.Put(cancellationToken, id, standardCategoryDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.StandardCategoryDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await standardCategoryService.Delete(cancellationToken, id));
    }
}

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
    public class FormStandardController : BaseController
    {
        private readonly IFormStandardService formStandardService;

        public FormStandardController(IFormStandardService formStandardService)
        {
           this.formStandardService = formStandardService;
        }
        [HttpPost]
        [HasPermission(PermissionEnum.FormStandardGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await formStandardService.GetPaginateList(cancellationToken, filter));


        [HttpGet]
        [HasPermission(PermissionEnum.FormStandardGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await formStandardService.GetListAsync(cancellationToken));


        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.FormStandardGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await formStandardService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.FormStandardAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] FormStandardDTO formStandardDTO) => Ok(await formStandardService.PostAsync(cancellationToken, formStandardDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.FormStandardUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] FormStandardDTO formStandardDTO) => Ok(await formStandardService.Put(cancellationToken, id, formStandardDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.FormStandardDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await formStandardService.Delete(cancellationToken, id));
    }
}

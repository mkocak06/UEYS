using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SpecificEducationController : BaseController
    {
        private readonly ISpecificEducationService specificEducationService;

        public SpecificEducationController(ISpecificEducationService specificEducationService)
        {
            this.specificEducationService = specificEducationService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.SpecificEducationPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await specificEducationService.GetPaginateList(cancellationToken, filter));

        //[HttpGet]
        //[HasPermission(PermissionEnum.SpecificEducationGetList)]
        //public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await specificEducationService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.SpecificEducationGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await specificEducationService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.SpecificEducationAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] SpecificEducationDTO specificEducationDTO) => Ok(await specificEducationService.PostAsync(cancellationToken, specificEducationDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.SpecificEducationUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] SpecificEducationDTO specificEducationDTO) => Ok(await specificEducationService.Put(cancellationToken, id, specificEducationDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.SpecificEducationDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await specificEducationService.Delete(cancellationToken, id));
    }
}

using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading.Tasks;
using System.Threading;
using Shared.Types;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdvisorThesisController : ControllerBase
    {
        private readonly IAdvisorThesisService advisorThesisService;

        public AdvisorThesisController(IAdvisorThesisService advisorThesisService)
        {
            this.advisorThesisService = advisorThesisService;
        }

        //[HttpGet("{thesisId}")]
        //[HasPermission(Shared.Types.PermissionEnum.AdvisorThesisGetList)]
        //public async Task<IActionResult> GetListByThesisId(CancellationToken cancellationToken, long thesisId) => Ok(await advisorThesisService.GetListByThesisIdAsync(cancellationToken, thesisId));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.AdvisorThesisGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await advisorThesisService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.AdvisorThesisAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] AdvisorThesisDTO advisorThesisDTO) => Ok(await advisorThesisService.PostAsync(cancellationToken, advisorThesisDTO));

        [HttpPost]
        [HasPermission(PermissionEnum.AdvisorThesisAdd)]
        public async Task<IActionResult> AddNotEducatorAdvisor(CancellationToken cancellationToken, [FromBody] AdvisorThesisDTO advisorThesisDTO) => Ok(await advisorThesisService.AddNotEducatorAdvisor(cancellationToken, advisorThesisDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.AdvisorThesisDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await advisorThesisService.Delete(cancellationToken, id));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.AdvisorThesisAdd)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] AdvisorThesisDTO advisorThesisDTO) => Ok(await advisorThesisService.Put(cancellationToken, id, advisorThesisDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.AdvisorThesisAdd)]
        public async Task<IActionResult> ChangeCoordinatorAdvisor(CancellationToken cancellationToken, int id, [FromBody] ChangeCoordinatorAdvisorThesisDTO advisorThesisDTO) => Ok(await advisorThesisService.ChangeCoordinatorAdvisor(cancellationToken, id, advisorThesisDTO));

    }
}

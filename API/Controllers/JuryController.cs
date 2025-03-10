using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JuryController : ControllerBase
    {
        private readonly IJuryService juryService;

        public JuryController(IJuryService juryService)
        {
            this.juryService = juryService;
        }

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.JuryGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await juryService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.JuryAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] JuryDTO juryDTO) => Ok(await juryService.PostAsync(cancellationToken, juryDTO));

        [HttpDelete]
        [HasPermission(Shared.Types.PermissionEnum.JuryDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long educatorId, long thesisId) => Ok(await juryService.Delete(cancellationToken, educatorId, thesisId));
    }
}

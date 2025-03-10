using Application.Interfaces;
using Application.Services;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DependentProgramController : ControllerBase
    {
        private readonly IDependentProgramService dependentProgramService;

        public DependentProgramController(IDependentProgramService dependentProgramService)
        {
            this.dependentProgramService = dependentProgramService;
        }
        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] DependentProgramDTO dependentProgramDTO) => Ok(await dependentProgramService.PostAsync(cancellationToken, dependentProgramDTO));
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] DependentProgramDTO dependentProgramDTO) => Ok(await dependentProgramService.Put(cancellationToken, id, dependentProgramDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ProtocolProgramUpdate)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await dependentProgramService.Delete(cancellationToken, id));

    }
}

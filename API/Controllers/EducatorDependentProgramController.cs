using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EducatorDependentProgramController : ControllerBase
    {
        private readonly IEducatorDependentProgramService educatorDependentProgramService;

        public EducatorDependentProgramController(IEducatorDependentProgramService educatorDependentProgramService)
        {
            this.educatorDependentProgramService = educatorDependentProgramService;
        }

        //[HttpGet("{dependProgId}")]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long dependProgId) => Ok(await educatorDependentProgramService.GetListByDependProgIdAsync(cancellationToken, dependProgId));

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] EducatorDependentProgramDTO educatorDependentProgramDTO) => Ok(await educatorDependentProgramService.Put(cancellationToken, id, educatorDependentProgramDTO));

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EducatorDependentProgramDTO educatorDependentProgramDTO) => Ok(await educatorDependentProgramService.PostAsync(cancellationToken, educatorDependentProgramDTO));

        [HttpDelete]
        [HasPermission(Shared.Types.PermissionEnum.ProtocolProgramUpdate)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long educatorId, long dependentProgramId) => Ok(await educatorDependentProgramService.Delete(cancellationToken, educatorId, dependentProgramId));
    }
}

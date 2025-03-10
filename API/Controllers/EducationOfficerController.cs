using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EducationOfficerController : ControllerBase
    {
        private readonly IEducationOfficerService educationOfficerService;

        public EducationOfficerController(IEducationOfficerService educationOfficerService)
        {
            this.educationOfficerService = educationOfficerService;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramGetById)]
        public async Task<IActionResult> GetPaginateListForProgramDetail(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educationOfficerService.GetPaginateListForProgramDetail(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramChangeOfficer)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EducationOfficerDTO educationOfficerDTO) => Ok(await educationOfficerService.PostAsync(cancellationToken, educationOfficerDTO));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgramChangeOfficer)]
        public async Task<IActionResult> ChangeProgramManager(CancellationToken cancellationToken, [FromBody] EducationOfficerDTO educationOfficerDTO) => Ok(await educationOfficerService.ChangeProgramManager(cancellationToken, educationOfficerDTO));

        [HttpGet]
        [HasPermission(PermissionEnum.StudentUpdate)]
        public async Task<IActionResult> GetListByProgramId(CancellationToken cancellationToken, long programId) => Ok(await educationOfficerService.GetListByProgramId(cancellationToken, programId));
    }
}

using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EducatorExpertiseBranchController : ControllerBase
    {
        private readonly IEducatorExpertiseBranchService educatorExpertiseBranchService;

        public EducatorExpertiseBranchController(IEducatorExpertiseBranchService educatorExpertiseBranchService)
        {
            this.educatorExpertiseBranchService = educatorExpertiseBranchService;
        }

        //[HttpGet("{educatorId}")]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long educatorId) => Ok(await educatorExpertiseBranchService.GetListByEducatorIdAsync(cancellationToken, educatorId));

        //[HttpPost]
        //public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EducatorExpertiseBranchDTO educatorExpertiseBranchDTO) => Ok(await educatorExpertiseBranchService.PostAsync(cancellationToken, educatorExpertiseBranchDTO));

        [HttpDelete]
        //[HasPermission(Shared.Types.PermissionEnum.)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await educatorExpertiseBranchService.Delete(cancellationToken, id));
    }
}

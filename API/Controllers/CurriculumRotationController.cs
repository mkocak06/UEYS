using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurriculumRotationController : ControllerBase
    {
        private readonly ICurriculumRotationService curriculumRotationService;

        public CurriculumRotationController(ICurriculumRotationService curriculumRotationService)
        {
            this.curriculumRotationService = curriculumRotationService;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumGetById)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await curriculumRotationService.GetPaginateList(cancellationToken, filter));


        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.CurriculumRotationGetById)]
        //public async Task<IActionResult> Get(CancellationToken cancellationToken, long id) => Ok(await curriculumRotationService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumUpdate)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] CurriculumRotationDTO curriculumRotationDTO) => Ok(await curriculumRotationService.PostAsync(cancellationToken, curriculumRotationDTO));

        //[HttpPut("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.CurriculumRotationPut)]
        //public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] CurriculumRotationDTO curriculumRotationDTO) => Ok(await curriculumRotationService.Put(cancellationToken, id, curriculumRotationDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumUpdate)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await curriculumRotationService.Delete(cancellationToken, id));
    }
}

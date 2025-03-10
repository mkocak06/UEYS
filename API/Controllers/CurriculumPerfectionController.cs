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
    public class CurriculumPerfectionController : ControllerBase
    {
        private readonly ICurriculumPerfectionService curriculumPerfectionService;

        public CurriculumPerfectionController(ICurriculumPerfectionService curriculumPerfectionService)
        {
            this.curriculumPerfectionService = curriculumPerfectionService;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumGetById)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await curriculumPerfectionService.GetPaginateList(cancellationToken, filter));


        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumGetById)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, long id) => Ok(await curriculumPerfectionService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumUpdate)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] CurriculumPerfectionDTO curriculumPerfectionDTO) => Ok(await curriculumPerfectionService.PostAsync(cancellationToken, curriculumPerfectionDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] CurriculumPerfectionDTO curriculumPerfectionDTO) => Ok(await curriculumPerfectionService.Put(cancellationToken, id, curriculumPerfectionDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.CurriculumUpdate)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await curriculumPerfectionService.Delete(cancellationToken, id));
    }
}

using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudyController : BaseController
    {
        private readonly IStudyService studyService;

        public StudyController(IStudyService studyService)
        {
            this.studyService = studyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await studyService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.StudyGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await studyService.GetByIdAsync(cancellationToken, id));
        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.StudyGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studyService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.StudyAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudyDTO studyDTO) => Ok(await studyService.PostAsync(cancellationToken, studyDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.StudyUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StudyDTO studyDTO) => Ok(await studyService.Put(cancellationToken, id, studyDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.StudyDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await studyService.Delete(cancellationToken, id));
    }
}

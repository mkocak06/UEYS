using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProfessionController : BaseController
    {
        private readonly IProfessionService facultyService;

        public ProfessionController(IProfessionService facultyService)
        {
            this.facultyService = facultyService;
        }

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.ProfessionGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await facultyService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ProfessionGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await facultyService.GetByIdAsync(cancellationToken, id));

        //[HttpGet("{uniId}")]
        //[HasPermission(Shared.Types.PermissionEnum.ProfessionGetByUniversityId)]
        //public async Task<IActionResult> GetByUniversityIdAsync(CancellationToken cancellationToken, long uniId) => Ok(await facultyService.GetByUniversityIdAsync(cancellationToken, uniId));
        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ProfessionGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await facultyService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ProfessionAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ProfessionDTO facultyDTO) => Ok(await facultyService.PostAsync(cancellationToken, facultyDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ProfessionUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ProfessionDTO facultyDTO) => Ok(await facultyService.Put(cancellationToken, id, facultyDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ProfessionDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await facultyService.Delete(cancellationToken, id));
    }
}

using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FacultyController : ControllerBase
    {
        private readonly IFacultyService facultyService;

        public FacultyController(IFacultyService facultyService)
        {
            this.facultyService = facultyService;
        }

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.FacultyGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await facultyService.GetPaginateList(cancellationToken, filter));

        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.FacultyGetList)]
        public async Task<IActionResult> GetList(CancellationToken cancellationToken) => Ok(await facultyService.GetListAsync(cancellationToken));

        [HttpGet("{uniId}")]
        //[HasPermission(Shared.Types.PermissionEnum.FacultyGetList)]
        public async Task<IActionResult> GetListByUniversityId(CancellationToken cancellationToken, long uniId) => Ok(await facultyService.GetListByUniversityId(cancellationToken, uniId));
    }
}

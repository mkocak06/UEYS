using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentPerfectionController : ControllerBase
    {
        private readonly IStudentPerfectionService studentPerfectionService;

        public StudentPerfectionController(IStudentPerfectionService studentPerfectionService)
        {
            this.studentPerfectionService = studentPerfectionService;
        }
        [HttpGet]
        [HasPermission(PermissionEnum.StudentPerfectionGetById)]
        public async Task<IActionResult> GetByStudentIdAsync(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType) => Ok(await studentPerfectionService.GetByStudentIdAsync(cancellationToken, studentId, perfectionType));

        [HttpGet]
        [HasPermission(PermissionEnum.StudentPerfectionGetById)]
        public async Task<IActionResult> GetListByStudentIdWithoutType(CancellationToken cancellationToken, long studentId) => Ok(await studentPerfectionService.GetListByStudentIdWithoutType(cancellationToken, studentId));

        [HttpGet]
        [HasPermission(PermissionEnum.StudentPerfectionGetById)]
        public async Task<IActionResult> GetByStudentAndPerfectionId(CancellationToken cancellationToken, long studentId, long perfectionId) => Ok(await studentPerfectionService.GetByStudentAndPerfectionId(cancellationToken, studentId, perfectionId));

        //[HttpGet("{id}")]
        //[HasPermission(PermissionEnum.StudentPerfectionGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await studentPerfectionService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentPerfectionAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudentPerfectionDTO studentPerfectionDTO) => Ok(await studentPerfectionService.PostAsync(cancellationToken, studentPerfectionDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentPerfectionUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StudentPerfectionDTO studentPerfectionDTO) => Ok(await studentPerfectionService.Put(cancellationToken, id, studentPerfectionDTO));

        [HttpDelete]
        [HasPermission(PermissionEnum.StudentPerfectionDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long studentId, long perfectionId) => Ok(await studentPerfectionService.Delete(cancellationToken, studentId, perfectionId));

        //[HttpPost]
        //[HasPermission(PermissionEnum.StudentPerfectioniCompleteAll)]
        //public async Task<IActionResult> CompleteAllPerfections(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType) => Ok(await studentPerfectionService.CompleteAllPerfections(cancellationToken, studentId, perfectionType));
    }
}

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
    public class StudentRotationPerfectionController : ControllerBase
    {
        private readonly IStudentRotationPerfectionService studentRotationPerfectionService;

        public StudentRotationPerfectionController(IStudentRotationPerfectionService StudentRotationPerfectionService)
        {
            this.studentRotationPerfectionService = StudentRotationPerfectionService;
        }

        [HttpGet]
        [HasPermission(PermissionEnum.StudentPerfectionGetById)]
        public async Task<IActionResult> GetByStrIdAndPerfectionId(CancellationToken cancellationToken, long studentRotationId, long perfectionId) => Ok(await studentRotationPerfectionService.GetByStrIdAndPerfectionId(cancellationToken, studentRotationId, perfectionId));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentPerfectionAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudentRotationPerfectionDTO StudentRotationPerfectionDTO) => Ok(await studentRotationPerfectionService.PostAsync(cancellationToken, StudentRotationPerfectionDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentPerfectionUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StudentRotationPerfectionDTO StudentRotationPerfectionDTO) => Ok(await studentRotationPerfectionService.Put(cancellationToken, id, StudentRotationPerfectionDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.StudentPerfectionDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await studentRotationPerfectionService.Delete(cancellationToken, id));
    }
}

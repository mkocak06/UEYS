using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Koru;
using Shared.FilterModels.Base;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RotationController : ControllerBase
    {
        private readonly IRotationService rotationService;

        public RotationController(IRotationService rotationService)
        {
            this.rotationService = rotationService;
        }

        //[HttpGet("{curriculumId}")]
        //[HasPermission(Shared.Types.PermissionEnum.RotationGetListPagination)]
        //public async Task<IActionResult> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId) => Ok(await rotationService.GetListByCurriculumId(cancellationToken, curriculumId));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.StudentRotationGetListByStudentId)]
        public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await rotationService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.StudentRotationGetListByStudentId)]
        public async Task<IActionResult> GetFormerStudentListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await rotationService.GetFormerStudentListByStudentId(cancellationToken, studentId));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.RotationGetListPagination)]
        //public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await rotationService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.RotationGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await rotationService.GetByIdAsync(cancellationToken, id));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.RotationAdd)]
        //public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] RotationDTO rotationDTO) => Ok(await rotationService.PostAsync(cancellationToken, rotationDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.RotationUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] RotationDTO rotationDTO) => Ok(await rotationService.Put(cancellationToken, id, rotationDTO));

        //[HttpDelete("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.RotationDelete)]
        //public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await rotationService.Delete(cancellationToken, id));

        [HttpPost]
        public async Task<IActionResult> UploadExcel(CancellationToken cancellationToken, IFormFile formFile) => Ok(await rotationService.UploadExcel(cancellationToken, formFile));
    }
}

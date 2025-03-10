using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Http;
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
    public class PerfectionController : ControllerBase
    {
        private readonly IPerfectionService perfectionService;

        public PerfectionController(IPerfectionService perfectionService)
        {
            this.perfectionService = perfectionService;
        }

        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.PerfectionGetList)]
        //public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await perfectionService.GetListAsync(cancellationToken));

        //[HttpGet("{curriculumId}")]
        //[HasPermission(Shared.Types.PermissionEnum.PerfectionGetList)]
        //public async Task<IActionResult> GetListByCurriculumId(CancellationToken cancellationToken, long curriculumId) => Ok(await perfectionService.GetListByCurriculumId(cancellationToken, curriculumId));


        [HttpGet]
        [HasPermission(PermissionEnum.StudentPerfectionGetListByStudentId)]
        public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId, PerfectionType perfectionType) => Ok(await perfectionService.GetListByStudentId(cancellationToken, studentId, perfectionType));

        [HttpPost]
        //[HasPermission(PermissionEnum.PerfectionGetListPagination)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await perfectionService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.PerfectionGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await perfectionService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.PerfectionAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] PerfectionDTO perfectionDTO) => Ok(await perfectionService.PostAsync(cancellationToken, perfectionDTO));

        [HttpPost]
        [HasPermission(PermissionEnum.PerfectionAdd)]
        public async Task<IActionResult> PostRotationPerfection(CancellationToken cancellationToken, long curriculumId, long rotationId, [FromBody] PerfectionDTO perfectionDTO) => Ok(await perfectionService.PostCurriculumRotationPerfection(cancellationToken, curriculumId, rotationId, perfectionDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.PerfectionUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] PerfectionDTO perfectionDTO) => Ok(await perfectionService.Put(cancellationToken, id, perfectionDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.PerfectionDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await perfectionService.Delete(cancellationToken, id));

        //[HttpPost]
        //public async Task<IActionResult> UploadExcel(CancellationToken cancellationToken, IFormFile formFile) => Ok(await perfectionService.UploadExcel(cancellationToken, formFile));

        //[HttpPost]
        //public async Task<IActionResult> ImportPerfectionExcel(CancellationToken cancellationToken, IFormFile formFile) => Ok(await perfectionService.ImportPerfectionList(cancellationToken, formFile));
        
        // [HttpPost]
        // public async Task<IActionResult> CurriculumPerfectionsImport(CancellationToken cancellationToken, IFormFile formFile) => Ok(await perfectionService.CurriculumPerfectionsImport(cancellationToken, formFile));
        // [HttpPost]
        // public async Task<IActionResult> CurriculumRotationPerfectionsImport(CancellationToken cancellationToken, IFormFile formFile) => Ok(await perfectionService.CurriculumRotationPerfectionsImport(cancellationToken, formFile));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.PerfectionExportExcelList)]
        public async Task<IActionResult> ExcelExportClinical(CancellationToken cancellationToken, long studentId) => Ok(await perfectionService.ExcelExportClinical(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.PerfectionExportExcelList)]
        public async Task<IActionResult> ExcelExportInterventional(CancellationToken cancellationToken, long studentId) => Ok(await perfectionService.ExcelExportInterventional(cancellationToken, studentId));
    }
}

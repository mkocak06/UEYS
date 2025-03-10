using Application.Interfaces;
using Core.Interfaces;
using Koru;
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
    public class StudentController : BaseController
    {
        private readonly IStudentService studentService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public StudentController(IStudentService studentService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.studentService = studentService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        [HttpGet]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> GetListForBreadCrumb(CancellationToken cancellationToken) => Ok(await studentService.GetListForBreadCrumb(cancellationToken));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.GetPaginateListOnly(cancellationToken, filter));

        [HttpGet]
        [HasPermission(PermissionEnum.StudentGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id, bool isDeleted = false) => Ok(await studentService.GetByIdAsync(cancellationToken, id, isDeleted));

        [HttpGet]
        [HasPermission(PermissionEnum.StudentGetById)]
        public async Task<IActionResult> GetRegistrationStudentById(CancellationToken cancellationToken, long id) => Ok(await studentService.GetRegistrationStudentById(cancellationToken, id));

        [HttpGet("{programId}")]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> GetListByProgramId(CancellationToken cancellationToken, long programId) => Ok(await studentService.GetListByProgramId(cancellationToken, programId));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudentDTO studentDTO) => Ok(await studentService.PostAsync(cancellationToken, studentDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StudentDTO studentDTO) => Ok(await studentService.Put(cancellationToken, id, studentDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.StudentDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id, string reason) => Ok(await studentService.Delete(cancellationToken, id, reason));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentDelete)]
        public async Task<IActionResult> CompletelyDelete(CancellationToken cancellationToken, int id) => Ok(await studentService.CompletelyDelete(cancellationToken, id));
        [HttpGet]
        [HasPermission(PermissionEnum.StudentGetList)]
        public ActionResult GetCountsByMonthsResponse() => Ok(studentService.GetCountsByMonthsResponse());

        [HttpGet]
        public async Task<ActionResult> GetRestartStudents(CancellationToken cancellationToken) => Ok(await studentService.GetRestartStudents(cancellationToken));

        [HttpGet]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<ActionResult> GetExpiredStudents(CancellationToken cancellationToken) => Ok(await studentService.GetExpiredStudents(cancellationToken));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.StudentDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.StudentDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentExcelExport)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.ExcelExport(cancellationToken, filter));

        [HttpPost]
        //[HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.CountByUniversityType(cancellationToken, filter));

        [HttpPost]
        //[HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> CountByExamType(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.CountByExamType(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> CountByQuotas(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.CountByQuotas(cancellationToken, filter));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> CountByProfession(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.CountByProfession(cancellationToken, filter));

        [HttpPost]
        //[HasPermission(PermissionEnum.StudentGetList)]
        public async Task<IActionResult> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter) => Ok(await studentService.CountsByProfessionInstitution(cancellationToken, filter));

    }
}

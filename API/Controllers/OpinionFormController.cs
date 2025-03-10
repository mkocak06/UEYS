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
    public class OpinionFormController : ControllerBase
    {
        private readonly IOpinionFormService opinionFormService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public OpinionFormController(IOpinionFormService opinionFormService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.opinionFormService = opinionFormService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        [HttpPost]
        [HasPermission(PermissionEnum.OpinionFormGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await opinionFormService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{id}")]
        //[HasPermission(PermissionEnum.OpinionFormGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await opinionFormService.GetByIdAsync(cancellationToken, id));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.OpinionFormGetList)]
        public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await opinionFormService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.OpinionFormAdd)]
        public async Task<IActionResult> GetStartAndEndDates(CancellationToken cancellationToken, long studentId) => Ok(await opinionFormService.GetStartAndEndDates(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.OpinionFormGetList)]
        public async Task<IActionResult> GetCanceledListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await opinionFormService.GetCanceledListByStudentId(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.OpinionFormGetList)]
        public async Task<IActionResult> CheckNegativeOpinions(CancellationToken cancellationToken, long studentId) => Ok(await opinionFormService.CheckNegativeOpinions(cancellationToken, studentId));

        [HttpPost]
        [HasPermission(PermissionEnum.OpinionFormAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] OpinionFormDTO opinionFormDTO) => Ok(await opinionFormService.PostAsync(cancellationToken, opinionFormDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.OpinionFormUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] OpinionFormDTO opinionFormDTO) => Ok(await opinionFormService.Put(cancellationToken, id, opinionFormDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.OpinionFormDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await opinionFormService.Delete(cancellationToken, id));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.OpinionFormDelete)]
        public async Task<IActionResult> Cancellation(CancellationToken cancellationToken, int id) => Ok(await opinionFormService.Cancellation(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.OpinionFormDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.OpinionFormDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.OpinionFormDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));


        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.OpinionFormExampleDownload)]
        public async Task<IActionResult> ExampleOpinionForm(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

    }
}

using Application.Interfaces;
using Application.Services;
using Core.Interfaces;
using Infrastructure.Services;
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
    public class ThesisController : ControllerBase
    {
        private readonly IThesisService thesisService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public ThesisController(IThesisService thesisService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.thesisService = thesisService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }
        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ThesisGetPaginateList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await thesisService.GetPaginateList(cancellationToken, filter));
        
        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ThesisGetById)]
        public async Task<IActionResult> GetByIdAsync(CancellationToken cancellationToken, long id) => Ok(await thesisService.GetByIdAsync(cancellationToken, id));

        //[HttpGet("{studentId}")]
        //[HasPermission(Shared.Types.PermissionEnum.ThesisGetByStudentId)]
        //public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await thesisService.GetListByStudentId(cancellationToken, studentId));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ThesisAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ThesisDTO thesisDTO) => Ok(await thesisService.PostAsync(cancellationToken, thesisDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ThesisUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ThesisDTO thesisDTO) => Ok(await thesisService.Put(cancellationToken, id, thesisDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.ThesisDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await thesisService.Delete(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.ThesisDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.ThesisDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.ThesisDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

using Application.Interfaces;
using Core.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using Shared.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ThesisDefenceController : BaseController
    {
        private readonly IThesisDefenceService thesisDefenceService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public ThesisDefenceController(IThesisDefenceService thesisDefenceService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.thesisDefenceService = thesisDefenceService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        [HttpGet("{thesisId}")]
        [HasPermission(PermissionEnum.ThesisDefenceGetListByThesisId)]
        public async Task<IActionResult> GetListByThesisId(CancellationToken cancellationToken, long thesisId) => Ok(await thesisDefenceService.GetListByThesisId(cancellationToken, thesisId));

        [HttpGet]
        [HasPermission(PermissionEnum.ThesisDefenceAdd)]
        public async Task<IActionResult> IsThesisDefenceAddable(CancellationToken cancellationToken, long thesisId, DateTime? date) => Ok(await thesisDefenceService.IsThesisDefenceAddable(cancellationToken, thesisId, date));

        //[HttpPost]
        //[HasPermission(PermissionEnum.ThesisDefenceGetPaginateList)]
        //public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await thesisDefenceService.GetPaginateList(cancellationToken, filter));

        [HttpGet]
        [HasPermission(PermissionEnum.ThesisDefenceGetById)]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken, long id) => Ok(await thesisDefenceService.GetById(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.ThesisDefenceAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ThesisDefenceDTO thesisDefenceDTO) => Ok(await thesisDefenceService.PostAsync(cancellationToken, thesisDefenceDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.ThesisDefenceUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ThesisDefenceDTO thesisDefenceDTO) => Ok(await thesisDefenceService.Put(cancellationToken, id, thesisDefenceDTO));

        [HttpGet]
        [HasPermission(PermissionEnum.ThesisDefenceDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id, long studentId) => Ok(await thesisDefenceService.Delete(cancellationToken, id, studentId));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.ThesisDefenceDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.ThesisDefenceDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.ThesisDefenceDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

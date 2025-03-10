using Application.Interfaces;
using Core.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScientificStudyController : ControllerBase
    {
        private readonly IScientificStudyService scientificStudyService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public ScientificStudyController(IScientificStudyService scientificStudyService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.scientificStudyService = scientificStudyService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.ScientificStudyGetList)]
        public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await scientificStudyService.GetListByStudentId(cancellationToken, studentId));

        [HttpPost]
        [HasPermission(PermissionEnum.ScientificStudyGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await scientificStudyService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.ScientificStudyGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await scientificStudyService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.ScientificStudyAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ScientificStudyDTO scientificStudyDTO) => Ok(await scientificStudyService.PostAsync(cancellationToken, scientificStudyDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.ScientificStudyUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ScientificStudyDTO scientificStudyDTO) => Ok(await scientificStudyService.Put(cancellationToken, id, scientificStudyDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.ScientificStudyDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await scientificStudyService.Delete(cancellationToken, id));
        
        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.ScientificStudyDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.ScientificStudyDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.ScientificStudyDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

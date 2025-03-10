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
    public class PerformanceRatingController : ControllerBase
    {
        private readonly IPerformanceRatingService performanceRatingService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public PerformanceRatingController(IPerformanceRatingService performanceRatingService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.performanceRatingService = performanceRatingService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await performanceRatingService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.PerformanceRatingGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await performanceRatingService.GetByIdAsync(cancellationToken, id));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingGetList)]
        public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await performanceRatingService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet("{studentId}")]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingGetList)]
        public async Task<IActionResult> GetByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await performanceRatingService.GetByStudentId(cancellationToken, studentId));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] PerformanceRatingDTO performanceRatingDTO) => Ok(await performanceRatingService.PostAsync(cancellationToken, performanceRatingDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] PerformanceRatingDTO performanceRatingDTO) => Ok(await performanceRatingService.Put(cancellationToken, id, performanceRatingDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await performanceRatingService.Delete(cancellationToken, id));
        
        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.PerformanceRatingDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.PerformanceRatingDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

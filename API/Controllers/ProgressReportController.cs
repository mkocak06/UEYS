using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Services;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.Types;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProgressReportController : BaseController
    {
        private readonly IProgressReportService progressReportService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public ProgressReportController(IProgressReportService progressReportService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.progressReportService = progressReportService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        //[HttpGet("{thesisId}")]
        //[HasPermission(Shared.Types.PermissionEnum.ProgressReportGetListByThesisId)]
        //public async Task<IActionResult> GetListByThesisId(CancellationToken cancellationToken, long thesisId) => Ok(await progressReportService.GetListByThesisId(cancellationToken, thesisId));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.ProgressReportGetPaginateList)]
        //public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await progressReportService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.ProgressReportGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await progressReportService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgressReportAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ProgressReportDTO progressReportDTO) => Ok(await progressReportService.PostAsync(cancellationToken, progressReportDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.ProgressReportUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] ProgressReportDTO progressReportDTO) => Ok(await progressReportService.Put(cancellationToken, id, progressReportDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.ProgressReportDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await progressReportService.Delete(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.ProgressReportDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpGet()]
        [HasPermission(PermissionEnum.ProgressReportAdd)]
        public async Task<IActionResult> CalculateStartEndDates(CancellationToken cancellationToken, long thesisId, long studentId) => Ok(await progressReportService.CalculateStartEndDates(cancellationToken, thesisId, studentId));

        [HttpPost]
        [HasPermission(PermissionEnum.ProgressReportDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.ProgressReportDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

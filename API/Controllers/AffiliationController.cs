using Application.Interfaces;
using Application.Services;
using Core.Interfaces;
using Infrastructure.Services;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AffiliationController : ControllerBase
    {
        private readonly IAffiliationService affiliationService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public AffiliationController(IAffiliationService affiliationService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.affiliationService = affiliationService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        [HttpGet]
        //[HasPermission(PermissionEnum.AffiliationGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await affiliationService.GetListAsync(cancellationToken));

        [HttpPost]
        //[HasPermission(PermissionEnum.AffiliationGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await affiliationService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{uniId}")]
        //[HasPermission(PermissionEnum.AffiliationGetListByUniversityId)]
        //public async Task<IActionResult> GetListByUniversityId(CancellationToken cancellationToken, long uniId) => Ok(await affiliationService.GetListByUniversityId(cancellationToken, uniId));

        //[HttpGet("{hospitalId}")]
        //[HasPermission(PermissionEnum.AffiliationGetListByHospitalId)]
        //public async Task<IActionResult> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId) => Ok(await affiliationService.GetListByHospitalId(cancellationToken, hospitalId));

        [HttpGet]
        [HasPermission(PermissionEnum.ProgramGetById)]
        public async Task<IActionResult> GetByAffiliation(CancellationToken cancellationToken, long facultyId, long hospitalId) => Ok(await affiliationService.GetByAffiliation(cancellationToken, facultyId, hospitalId));

        [HttpGet]
        [HasPermission(PermissionEnum.AffiliationGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id, DocumentTypes docType) => Ok(await affiliationService.GetByIdAsync(cancellationToken, id, docType));

        [HttpPost]
        [HasPermission(PermissionEnum.AffiliationAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] AffiliationDTO affiliationDTO) => Ok(await affiliationService.PostAsync(cancellationToken, affiliationDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.AffiliationUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] AffiliationDTO affiliationDTO) => Ok(await affiliationService.Put(cancellationToken, id, affiliationDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.AffiliationDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await affiliationService.Delete(cancellationToken, id));

        [HttpPost]
        [HasPermission(PermissionEnum.AffiliationExcelExport)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await affiliationService.ExcelExport(cancellationToken, filter));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.AffiliationDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.AffiliationDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.AffiliationDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

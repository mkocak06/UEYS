using Application.Interfaces;
using Core.Interfaces;
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
    public class EducatorController : ControllerBase
    {
        private readonly IEducatorService educatorService;
        private readonly IUniversityService universityService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public EducatorController(IEducatorService educatorService, IFileManagementService fileManagementService, IS3Service s3Service, IUniversityService universityService)
        {
            this.educatorService = educatorService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
            this.universityService = universityService;
        }

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await educatorService.GetListAsync(cancellationToken));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EducatorAdd)]
        public async Task<IActionResult> GetUniversityPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await universityService.GetPaginateList(cancellationToken, filter));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetPaginateListForAdvisor(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.GetPaginateListForAdvisor(cancellationToken, filter));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        //public async Task<IActionResult> GetPaginateListByCore(CancellationToken cancellationToken, FilterDTO filter, long studentId) => Ok(await educatorService.GetPaginateListByCore(cancellationToken, filter, studentId));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.GetPaginateListOnly(cancellationToken, filter));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetPaginateListByProgramId(CancellationToken cancellationToken, FilterDTO filter, long programId) => Ok(await educatorService.GetPaginateListByProgramId(cancellationToken, filter, programId));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetPaginateListByUniversityId(CancellationToken cancellationToken, FilterDTO filter, long uniId) => Ok(await educatorService.GetPaginateListByUniversityId(cancellationToken, filter, uniId));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetPaginateListByHospitalId(CancellationToken cancellationToken, FilterDTO filter, long hospitalId) => Ok(await educatorService.GetPaginateListByHospitalId(cancellationToken, filter, hospitalId));

        [HttpGet("{identityNo}")]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetById)]
        public async Task<IActionResult> GetByIdentityNo(CancellationToken cancellationToken, string identityNo) => Ok(await educatorService.GetByIdentityNo(cancellationToken, identityNo));

        [HttpGet("{uniId}")]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> GetListByUniversityId(CancellationToken cancellationToken, long uniId) => Ok(await educatorService.GetListByUniversityId(cancellationToken, uniId));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await educatorService.GetByIdAsync(cancellationToken, id));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.EducatorGetById)]
        public async Task<IActionResult> GetByIdForJuryList(CancellationToken cancellationToken, long id) => Ok(await educatorService.GetByIdForJuryList(cancellationToken, id));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.EducatorAdd)]
        //public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EducatorDTO educatorDTO) => Ok(await educatorService.PostAsync(cancellationToken, educatorDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.EducatorUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] EducatorDTO educatorDTO) => Ok(await educatorService.Put(cancellationToken, id, educatorDTO));

        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.EducatorDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.EducatorDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.EducatorDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

        [HttpPost]
        [HasPermission(PermissionEnum.EducatorExcelExport)]
        public async Task<IActionResult> ExcelExport(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.ExcelExport(cancellationToken, filter));
        [HttpPost]
        //[HasPermission(PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> CountByAcademicTitle(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.CountByAcademicTitle(cancellationToken, filter));
        [HttpPost]
        [HasPermission(PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> CountByProfession(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.CountByProfession(cancellationToken, filter));

        [HttpPost]
        //[HasPermission(PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.CountsByProfessionInstitution(cancellationToken, filter));


        [HttpPost]
        //[HasPermission(PermissionEnum.EducatorGetList)]
        public async Task<IActionResult> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educatorService.CountByUniversityType(cancellationToken, filter));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.EducatorGetById)]
        public async Task<IActionResult> WorkingLifeById(CancellationToken cancellationToken, long id) => Ok(await educatorService.WorkingLifeById(cancellationToken, id));
    }
}

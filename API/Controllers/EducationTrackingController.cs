using Application.Interfaces;
using Core.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EducationTrackingController : ControllerBase
    {
        private readonly IEducationTrackingService educationTrackingService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public EducationTrackingController(IEducationTrackingService educationTrackingService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.educationTrackingService = educationTrackingService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.EducationTrackingGetPaginateList)]
        //public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await educationTrackingService.GetPaginateList(cancellationToken, filter));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.EducationTrackingGetPaginateList)]
        public async Task<IActionResult> GetListByStudentIdAsync(CancellationToken cancellationToken, long studentId) => Ok(await educationTrackingService.GetListByStudentIdAsync(cancellationToken, studentId));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.EducationTrackingGetPaginateList)]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken, long id) => Ok(await educationTrackingService.GetById(cancellationToken, id));

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.EducationTrackingGetPaginateList)]
        public async Task<IActionResult> GetEducationStartByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await educationTrackingService.GetEducationStartByStudentId(cancellationToken, studentId));

        [HttpPut]
        public async Task<IActionResult> GetTimeIncreasingRecordsByDate(CancellationToken cancellationToken, [FromBody] OpinionFormRequestDTO opinionForm) => Ok(await educationTrackingService.GetTimeIncreasingRecordsByDate(cancellationToken, opinionForm));

        [HttpPut]
        public async Task<IActionResult> ReturnToMainProgramInProtocol(CancellationToken cancellationToken, long studentDependentProgramId, [FromBody] StudentDependentProgramPaginateDTO studentDependentProgram) => Ok(await educationTrackingService.ReturnToMainProgramInProtocol(cancellationToken, studentDependentProgramId, studentDependentProgram));

        [HttpPost]
        public async Task<IActionResult> GetRemainingDaysForDependentProgram(CancellationToken cancellationToken, [FromBody] StudentDependentProgramPaginateDTO studentDependentProgram) => Ok(await educationTrackingService.GetRemainingDaysForDependentProgram(cancellationToken, studentDependentProgram));

        [HttpPut]
        public async Task<IActionResult> SendStudentToDependentProgram(CancellationToken cancellationToken, long studentDependentProgramId, [FromBody] StudentDependentProgramPaginateDTO studentDependentProgram) => Ok(await educationTrackingService.SendStudentToDependentProgram(cancellationToken, studentDependentProgramId, studentDependentProgram));

        [HttpPost]
        [HasPermission(PermissionEnum.EducationTrackingAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EducationTrackingDTO educationTrackingDTO) => Ok(await educationTrackingService.PostAsync(cancellationToken, educationTrackingDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.EducationTrackingUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] EducationTrackingDTO educationTrackingDTO) => Ok(await educationTrackingService.Put(cancellationToken, id, educationTrackingDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.EducationTrackingDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await educationTrackingService.Delete(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.EducationTrackingDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.EducationTrackingDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.EducationTrackingDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

using Application.Interfaces;
using Core.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentRotationController : ControllerBase
    {
        private readonly IStudentRotationService studentRotationService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public StudentRotationController(IStudentRotationService studentRotationService, IS3Service s3Service, IFileManagementService fileManagementService)
        {
            this.studentRotationService = studentRotationService;
            this.s3Service = s3Service;
            this.fileManagementService = fileManagementService;
        }

        [HttpGet("{studentId}")]
        [HasPermission(PermissionEnum.StudentRotationGetListByStudentId)]
        public async Task<IActionResult> GetListByStudentId(CancellationToken cancellationToken, long studentId) => Ok(await studentRotationService.GetListByStudentId(cancellationToken, studentId));

        [HttpGet("{userId}")]
        [HasPermission(PermissionEnum.StudentRotationGetListByStudentId)]
        public async Task<IActionResult> GetFormerStudentsListByUserId(CancellationToken cancellationToken, long userId) => Ok(await studentRotationService.GetFormerStudentsListByUserId(cancellationToken, userId));

        [HttpGet("{id}")]
        [HasPermission(PermissionEnum.StudentRotationGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await studentRotationService.GetByIdAsync(cancellationToken, id));

        [HttpPut]
        [HasPermission(PermissionEnum.StudentRotationAdd)]
        public async Task<IActionResult> GetEndDateByStartDate(CancellationToken cancellationToken, [FromBody] StudentRotationDTO studentRotationDTO) => Ok(await studentRotationService.GetEndDateByStartDate(cancellationToken, studentRotationDTO));

        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.StudentRotationGetById)]
        //public async Task<IActionResult> GetByStudentAndRotationId(CancellationToken cancellationToken, long studentId, long rotationId) => Ok(await studentRotationService.GetByStudentAndRotationId(cancellationToken, studentId, rotationId));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentRotationAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] StudentRotationDTO studentRotationDTO) => Ok(await studentRotationService.PostAsync(cancellationToken, studentRotationDTO));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentRotationAdd)]
        public async Task<IActionResult> AddPastRotation(CancellationToken cancellationToken, [FromBody] StudentRotationDTO studentRotationDTO) => Ok(await studentRotationService.AddPastRotation(cancellationToken, studentRotationDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentRotationUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] StudentRotationDTO studentRotationDTO) => Ok(await studentRotationService.Put(cancellationToken, id, studentRotationDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentRotationUpdate)]
        public async Task<IActionResult> FinishStudentsRotation(CancellationToken cancellationToken, long id, [FromBody] StudentRotationDTO studentRotationDTO) => Ok(await studentRotationService.FinishStudentsRotation(cancellationToken, id, studentRotationDTO));

        [HttpPut]
        [HasPermission(PermissionEnum.StudentRotationUpdate)]
        public async Task<IActionResult> SendStudentToRotation(CancellationToken cancellationToken, [FromBody] StudentRotationDTO studentRotationDTO) => Ok(await studentRotationService.SendStudentToRotation(cancellationToken, studentRotationDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.StudentRotationDeletePast)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, long id) => Ok(await studentRotationService.Delete(cancellationToken, id));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.StudentRotationDelete)]
        public async Task<IActionResult> DeleteActiveRotation(CancellationToken cancellationToken, long id) => Ok(await studentRotationService.DeleteActiveRotation(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.StudentRotationDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.StudentRotationDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.StudentRotationDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

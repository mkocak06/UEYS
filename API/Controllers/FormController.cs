using Application.Interfaces;
using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.RequestModels;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Types;
using Application.Services;
using Infrastructure.Services;
using Core.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FormController : BaseController
    {
        private readonly IFormService formService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public FormController(IFormService formService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.formService = formService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.FormGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await formService.GetPaginateList(cancellationToken, filter));


        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.FormGetList)]
        public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await formService.GetListAsync(cancellationToken));

        [HttpGet("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.FormGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await formService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.FormAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] FormDTO formDTO) => Ok(await formService.PostAsync(cancellationToken, formDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.FormUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] FormDTO formDTO) => Ok(await formService.Put(cancellationToken, id, formDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.FormDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await formService.Delete(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.FormDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.FormDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.FormDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));




    }
}

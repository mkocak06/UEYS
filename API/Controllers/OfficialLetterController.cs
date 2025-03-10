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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OfficialLetterController : BaseController
    {
        private readonly IOfficialLetterService officialLetterService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public OfficialLetterController(IOfficialLetterService officialLetterService,  IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.officialLetterService = officialLetterService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
            
        }

        [HttpGet("{thesisId}")]
        [HasPermission(Shared.Types.PermissionEnum.OfficialLetterGetListByThesisId)]
        public async Task<IActionResult> GetListByThesisId(CancellationToken cancellationToken, long thesisId) => Ok(await officialLetterService.GetListByThesisId(cancellationToken, thesisId));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.OfficialLetterGetPaginateList)]
        //public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await officialLetterService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.OfficialLetterGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await officialLetterService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.OfficialLetterAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] OfficialLetterDTO officialLetterDTO) => Ok(await officialLetterService.PostAsync(cancellationToken, officialLetterDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.OfficialLetterUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] OfficialLetterDTO officialLetterDTO) => Ok(await officialLetterService.Put(cancellationToken, id, officialLetterDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.OfficialLetterDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await officialLetterService.Delete(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.OfficialLetterDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.OfficialLetterDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.OfficialLetterDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

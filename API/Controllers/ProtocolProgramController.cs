using Amazon.Runtime.Documents;
using Application.Interfaces;
using Application.Services;
using Core.Interfaces;
using Infrastructure.Services;
using Koru;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProtocolProgramController : ControllerBase
    {
        private readonly IProtocolProgramService protocolProgramService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;


        public ProtocolProgramController(IProtocolProgramService protocolProgramService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.protocolProgramService = protocolProgramService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        //[HttpGet("{progType}")]
        //[HasPermission(PermissionEnum.ProtocolProgramGetList)]
        //public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken, ProgramType progType) => Ok(await protocolProgramService.GetListAsync(cancellationToken, progType));

        [HttpPost]
        [HasPermission(PermissionEnum.ProtocolProgramGetList)]
        public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await protocolProgramService.GetPaginateList(cancellationToken, filter));

        [HttpGet]
        [HasPermission(PermissionEnum.ProtocolProgramGetById)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id, DocumentTypes docType, ProgramType progType) => Ok(await protocolProgramService.GetByIdAsync(cancellationToken, id, docType, progType));

        [HttpGet]
        [HasPermission(PermissionEnum.ProtocolProgramGetList)]
        public async Task<IActionResult> GetListByUniversityId(CancellationToken cancellationToken, long uniId, ProgramType progType) => Ok(await protocolProgramService.GetListByUniversityId(cancellationToken, uniId, progType));

        [HttpGet]
        [HasPermission(PermissionEnum.ProtocolProgramGetById)]
        public async Task<IActionResult> GetEducatorListForComplementProgram(CancellationToken cancellationToken, long programId) => Ok(await protocolProgramService.GetEducatorListForComplementProgram(cancellationToken, programId));

        [HttpGet]
        [HasPermission(PermissionEnum.ProgramGetList)]
        public async Task<IActionResult> GetByProgramId(CancellationToken cancellationToken, long programId, ProgramType progType) => Ok(await protocolProgramService.GetByProgramId(cancellationToken, programId, progType));

        [HttpPost]
        [HasPermission(PermissionEnum.ProtocolProgramAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] ProtocolProgramDTO protocolProgramDTO) => Ok(await protocolProgramService.PostAsync(cancellationToken, protocolProgramDTO));

        [HttpPut("{id}")]
        [HasPermission(PermissionEnum.ProtocolProgramUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, long id, [FromBody] ProtocolProgramResponseDTO protocolProgramDTO) => Ok(await protocolProgramService.Put(cancellationToken, id, protocolProgramDTO));

        [HttpDelete("{id}")]
        [HasPermission(PermissionEnum.ProtocolProgramDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await protocolProgramService.Delete(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(PermissionEnum.ProtocolProgramDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(PermissionEnum.ProtocolProgramDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.ProtocolProgramDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

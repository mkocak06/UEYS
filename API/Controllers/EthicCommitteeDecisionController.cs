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
    public class EthicCommitteeDecisionController : BaseController
    {
        private readonly IEthicCommitteeDecisionService ethicCommitteeDecisionService;
        private readonly IFileManagementService fileManagementService;
        private readonly IS3Service s3Service;

        public EthicCommitteeDecisionController(IEthicCommitteeDecisionService ethicCommitteeDecisionService, IFileManagementService fileManagementService, IS3Service s3Service)
        {
            this.ethicCommitteeDecisionService = ethicCommitteeDecisionService;
            this.fileManagementService = fileManagementService;
            this.s3Service = s3Service;
        }

        //[HttpGet("{thesisId}")]
        //[HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionGetListByThesisId)]
        //public async Task<IActionResult> GetListByThesisId(CancellationToken cancellationToken, long thesisId) => Ok(await ethicCommitteeDecisionService.GetListByThesisId(cancellationToken, thesisId));

        //[HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionGetPaginateList)]
        //public async Task<IActionResult> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter) => Ok(await ethicCommitteeDecisionService.GetPaginateList(cancellationToken, filter));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await ethicCommitteeDecisionService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] EthicCommitteeDecisionDTO ethicCommitteeDecisionDTO) => Ok(await ethicCommitteeDecisionService.PostAsync(cancellationToken, ethicCommitteeDecisionDTO));

        [HttpPut("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] EthicCommitteeDecisionDTO ethicCommitteeDecisionDTO) => Ok(await ethicCommitteeDecisionService.Put(cancellationToken, id, ethicCommitteeDecisionDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await ethicCommitteeDecisionService.Delete(cancellationToken, id));

        [HttpGet("{bucketKey}")]
        [HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionDocumentDownload)]
        public async Task<IActionResult> DownloadFile(CancellationToken cancellationToken, string bucketKey) => Ok(await s3Service.GetFileS3(cancellationToken, bucketKey));

        [HttpPost]
        [HasPermission(Shared.Types.PermissionEnum.EthicCommitteeDecisionDocumentUpload)]
        public async Task<IActionResult> UploadFile(CancellationToken cancellationToken, [FromForm] FileUploadModelDTO fileModel) => Ok(await fileManagementService.UploadFileToS3(cancellationToken, fileModel));

        [HttpDelete]
        [HasPermission(PermissionEnum.EthicCommitteeDecisionDocumentDelete)]
        public async Task<IActionResult> DeleteFileS3(CancellationToken cancellationToken, DocumentTypes documentType, long entityId) => Ok(await fileManagementService.DeleteFileS3(cancellationToken, documentType, entityId));

    }
}

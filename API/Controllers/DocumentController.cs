using Koru;
using Microsoft.AspNetCore.Mvc;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using Shared.Types;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DocumentController : BaseController
    {
        private readonly IDocumentService documentService;

        public DocumentController(IDocumentService documentService)
        {
            this.documentService = documentService;
        }
        
        [HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.DocumentGetByType)]
        public async Task<IActionResult> GetListByTypeByEntityAsync(CancellationToken cancellationToken, long entityId, DocumentTypes docType) => Ok(await documentService.GetListByTypeByEntityAsync(cancellationToken, entityId, docType));

        [HttpGet]
        [HasPermission(Shared.Types.PermissionEnum.DocumentGetDeletedList)]
        public async Task<IActionResult> GetDeletedList(CancellationToken cancellationToken) => Ok(await documentService.GetDeletedList(cancellationToken));

        //[HttpGet]
        //[HasPermission(Shared.Types.PermissionEnum.DocumentGetList)]
        //public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken) => Ok(await documentService.GetListAsync(cancellationToken));

        //[HttpGet("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.DocumentGetById)]
        //public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, long id) => Ok(await documentService.GetByIdAsync(cancellationToken, id));

        [HttpPost]
        //[HasPermission(Shared.Types.PermissionEnum.DocumentAdd)]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, [FromBody] DocumentDTO documentDTO) => Ok(await documentService.PostAsync(cancellationToken, documentDTO));

        [HttpPut("{id}")]
        //[HasPermission(Shared.Types.PermissionEnum.DocumentUpdate)]
        public async Task<IActionResult> Put(CancellationToken cancellationToken, int id, [FromBody] DocumentDTO documentDTO) => Ok(await documentService.Put(cancellationToken, id, documentDTO));

        [HttpDelete("{id}")]
        [HasPermission(Shared.Types.PermissionEnum.DocumentDelete)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id) => Ok(await documentService.Delete(cancellationToken, id));
    }
}

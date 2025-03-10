using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDocumentService
    {
        Task<ResponseWrapper<List<DocumentResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<DocumentResponseDTO>> UpdateServiceUrl(CancellationToken cancellationToken, long id, string serviceUrl);
        Task<ResponseWrapper<DocumentResponseDTO>> PostAsync(CancellationToken cancellationToken, DocumentDTO documentDTO);
        Task<ResponseWrapper<DocumentResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<DocumentResponseDTO>>> GetListByTypeByEntityAsync(CancellationToken cancellationToken, long entityId, DocumentTypes doctype);
        Task<ResponseWrapper<DocumentResponseDTO>> Put(CancellationToken cancellationToken, long id, DocumentDTO documentDTO);
        Task<ResponseWrapper<DocumentResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<DocumentResponseDTO>>> GetDeletedList(CancellationToken cancellationToken);
        Task<ResponseWrapper<bool>> UnDeleteDocument(CancellationToken cancellationToken, long id);
    }
}

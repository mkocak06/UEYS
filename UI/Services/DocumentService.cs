using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UI.Services;

public interface IDocumentService
{
    Task<ResponseWrapper<List<DocumentResponseDTO>>> GetList();
    Task<ResponseWrapper<List<DocumentResponseDTO>>> GetDeletedList();
    Task<ResponseWrapper<List<DocumentResponseDTO>>> GetListByTypeByEntity(long? entityId, DocumentTypes docType);
    Task<ResponseWrapper<DocumentResponseDTO>> Add(DocumentDTO documentDTO);
    Task<ResponseWrapper<DocumentResponseDTO>> GetById(long id);
    Task<ResponseWrapper<DocumentResponseDTO>> Update(long id, DocumentDTO documentDTO);
    Task Delete(long id);

}

public class DocumentService : IDocumentService
{
    private readonly IHttpService _httpService;
    public DocumentService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<DocumentResponseDTO>> Add(DocumentDTO documentDTO)
    {
        return await _httpService.Post<ResponseWrapper<DocumentResponseDTO>>($"Document/Post", documentDTO);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Document/Delete/{id}");
    }


    public Task<ResponseWrapper<DocumentResponseDTO>> GetById(long id)
    {
        throw new System.NotImplementedException();
    }

    public Task<ResponseWrapper<List<DocumentResponseDTO>>> GetList()
    {
        throw new System.NotImplementedException();
    }

    public async Task<ResponseWrapper<List<DocumentResponseDTO>>> GetDeletedList()
    {
        return await _httpService.Get<ResponseWrapper<List<DocumentResponseDTO>>>("Archive/GetDocumentList");
    }

    public async Task<ResponseWrapper<List<DocumentResponseDTO>>> GetListByTypeByEntity(long? entityId, DocumentTypes docType)
    {
        return await _httpService.Get<ResponseWrapper<List<DocumentResponseDTO>>>($"Document/GetListByTypeByEntity?entityId={entityId}&docType={docType}");
    }

    public async Task<ResponseWrapper<DocumentResponseDTO>> Update(long id, DocumentDTO documentDTO)
    {
        return await _httpService.Put<ResponseWrapper<DocumentResponseDTO>>($"Document/Put/{id}", documentDTO);
    }


}
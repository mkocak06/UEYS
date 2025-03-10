using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Types;

namespace UI.Services
{
    public interface IOfficialLetterService
    {
        Task<PaginationModel<OfficialLetterResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<OfficialLetterResponseDTO>> Add(OfficialLetterDTO OfficialLetter);
        Task<ResponseWrapper<OfficialLetterResponseDTO>> Update(long id, OfficialLetterDTO OfficialLetter);
        Task Delete(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class OfficialLetterService : IOfficialLetterService
    {
        private readonly IHttpService _httpService;

        public OfficialLetterService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<OfficialLetterResponseDTO>> Add(OfficialLetterDTO OfficialLetter)
        {
            return await _httpService.Post<ResponseWrapper<OfficialLetterResponseDTO>>($"OfficialLetter/Post", OfficialLetter);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"OfficialLetter/Delete/{id}");
        }

        public async Task<PaginationModel<OfficialLetterResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<OfficialLetterResponseDTO>>($"OfficialLetter/GetPaginateList", filter);
        }

        public async Task<ResponseWrapper<OfficialLetterResponseDTO>> Update(long id, OfficialLetterDTO OfficialLetter)
        {
            return await _httpService.Put<ResponseWrapper<OfficialLetterResponseDTO>>($"OfficialLetter/Put/{id}", OfficialLetter);
        }
        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"OfficialLetter/DownloadFile/{bucketKey}");
        }

        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"OfficialLetter/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}

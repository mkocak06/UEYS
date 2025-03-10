using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace UI.Services
{
    public interface IThesisService
    {
        Task<PaginationModel<ThesisResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<ThesisResponseDTO>> GetById(long id);
        Task<ResponseWrapper<List<ThesisResponseDTO>>> GetListByStudentId(long studentId);
        Task<ResponseWrapper<List<ThesisResponseDTO>>> GetForEReport(long studentId);
        Task<ResponseWrapper<ThesisResponseDTO>> PostAsync(ThesisDTO thesisDTO);
        Task<ResponseWrapper<ThesisResponseDTO>> Put(long id, ThesisDTO thesisDTO);
        Task Delete(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class ThesisService : IThesisService
    {
        private readonly IHttpService _httpService;

        public ThesisService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<List<ThesisResponseDTO>>> GetListByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<ThesisResponseDTO>>>($"Thesis/GetListByStudentId/{studentId}");
        }

        public async Task<ResponseWrapper<List<ThesisResponseDTO>>> GetForEReport(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<ThesisResponseDTO>>>($"EReport/GetThesis/{studentId}");
        }

        public async Task<ResponseWrapper<ThesisResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<ThesisResponseDTO>>($"Thesis/GetById/{id}");
        }
        public async Task<ResponseWrapper<ThesisResponseDTO>> PostAsync(ThesisDTO thesis)
        {
            return await _httpService.Post<ResponseWrapper<ThesisResponseDTO>>($"Thesis/Post", thesis);
        }

        public async Task<ResponseWrapper<ThesisResponseDTO>> Put(long id, ThesisDTO thesis)
        {
            return await _httpService.Put<ResponseWrapper<ThesisResponseDTO>>($"Thesis/Put/{id}", thesis);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"Thesis/Delete/{id}");
        }

        public async Task<PaginationModel<ThesisResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<ThesisResponseDTO>>($"Thesis/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"Thesis/DownloadFile/{bucketKey}");
        }
        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"Thesis/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}

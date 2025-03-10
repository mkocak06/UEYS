using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace UI.Services
{
    public interface IPerformanceRatingService
    {
        Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetAll();
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetById(long id);
        Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetListByStudentId(long studentId);
        Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetForEReport(long studentId);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetByStudentId(long studentId);
        Task<PaginationModel<PerformanceRatingResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> Add(PerformanceRatingDTO rating);
        Task<ResponseWrapper<PerformanceRatingResponseDTO>> Update(long id, PerformanceRatingDTO rating);
        Task Delete(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }
    public class PerformanceRatingService : IPerformanceRatingService
    {
        private readonly IHttpService _httpService;

        public PerformanceRatingService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<PaginationModel<PerformanceRatingResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<PerformanceRatingResponseDTO>>($"PerformanceRating/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetAll()
        {
            return await _httpService.Get<ResponseWrapper<List<PerformanceRatingResponseDTO>>>("PerformanceRating/GetList");
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<PerformanceRatingResponseDTO>>($"PerformanceRating/Get/{id}");
        }

        public async Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetListByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<PerformanceRatingResponseDTO>>>($"PerformanceRating/GetListByStudentId/{studentId}");
        }

        public async Task<ResponseWrapper<List<PerformanceRatingResponseDTO>>> GetForEReport(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<PerformanceRatingResponseDTO>>>($"EReport/GetPerformanceRating/{studentId}");
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> GetByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<PerformanceRatingResponseDTO>>($"PerformanceRating/GetByStudentId/{studentId}");
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> Add(PerformanceRatingDTO rating)
        {
            return await _httpService.Post<ResponseWrapper<PerformanceRatingResponseDTO>>($"PerformanceRating/Post", rating);
        }

        public async Task<ResponseWrapper<PerformanceRatingResponseDTO>> Update(long id, PerformanceRatingDTO rating)
        {
            return await _httpService.Put<ResponseWrapper<PerformanceRatingResponseDTO>>($"PerformanceRating/Put/{id}", rating);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"PerformanceRating/Delete/{id}");
        }
        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"PerformanceRating/DownloadFile/{bucketKey}");
        }
        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"PerformanceRating/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}

using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Services
{

    public interface IOpinionService
    {
        Task<PaginationModel<OpinionFormResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetListByStudentId(long studentId);
        Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetCanceledListByStudentId(long studentId);
        Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetForEReport(long studentId);
        Task<ResponseWrapper<OpinionFormResponseDTO>> GetStartAndEndDates(long studentId);
        Task<ResponseWrapper<OpinionFormResponseDTO>> GetById(long id);
        Task<ResponseWrapper<OpinionFormResponseDTO>> Add(OpinionFormDTO opinion);
        Task<ResponseWrapper<OpinionFormResponseDTO>> Update(long id, OpinionFormDTO opinion);
        Task Delete(long id);
        Task<ResponseWrapper<StudentStatusCheckDTO>> Cancellation(long id);
        Task<ResponseWrapper<FileResponseDTO>> DownloadExampleForm(string bucketKey);
        Task<ResponseWrapper<StudentStatusCheckDTO>> CheckNegativeOpinions(long studentId);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class OpinionService : IOpinionService
    {
        private readonly IHttpService _httpService;

        public OpinionService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<PaginationModel<OpinionFormResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<OpinionFormResponseDTO>>("OpinionForm/GetPaginateList", filter);
        }

        public async Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetListByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<OpinionFormResponseDTO>>>($"OpinionForm/GetListByStudentId/{studentId}");
        }

        public async Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetCanceledListByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<OpinionFormResponseDTO>>>($"OpinionForm/GetCanceledListByStudentId/{studentId}");
        }

        public async Task<ResponseWrapper<List<OpinionFormResponseDTO>>> GetForEReport(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<OpinionFormResponseDTO>>>($"EReport/GetOpinionForm/{studentId}");
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<OpinionFormResponseDTO>>($"OpinionForm/Get/{id}");
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> Add(OpinionFormDTO opinion)
        {
            return await _httpService.Post<ResponseWrapper<OpinionFormResponseDTO>>($"OpinionForm/Post", opinion);
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> Update(long id, OpinionFormDTO opinion)
        {
            return await _httpService.Put<ResponseWrapper<OpinionFormResponseDTO>>($"OpinionForm/Put/{id}", opinion);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"OpinionForm/Delete/{id}");
        }

        public async Task<ResponseWrapper<StudentStatusCheckDTO>> Cancellation(long id)
        {
            return await _httpService.Put<ResponseWrapper<StudentStatusCheckDTO>>($"OpinionForm/Cancellation/{id}", null);
        }

        public async Task<ResponseWrapper<StudentStatusCheckDTO>> CheckNegativeOpinions(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<StudentStatusCheckDTO>>($"OpinionForm/CheckNegativeOpinions/{studentId}");
        }

        public async Task<ResponseWrapper<OpinionFormResponseDTO>> GetStartAndEndDates(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<OpinionFormResponseDTO>>($"OpinionForm/GetStartAndEndDates/{studentId}");
        }

        public async Task<ResponseWrapper<FileResponseDTO>> DownloadExampleForm(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"OpinionForm/ExampleOpinionForm/{bucketKey}");
        }

        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"OpinionForm/DownloadFile/{bucketKey}");
        }

        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"OpinionForm/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}

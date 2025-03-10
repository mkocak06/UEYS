using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Types;

namespace UI.Services
{
    public interface IProgressReportService
    {
        Task<PaginationModel<ProgressReportResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<ProgressReportResponseDTO>> Add(ProgressReportDTO ProgressReport);
        Task<ResponseWrapper<ProgressReportResponseDTO>> Update(long id, ProgressReportDTO ProgressReport);
        Task Delete(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task<ResponseWrapper<ProgressReportResponseDTO>> CalculateStartEndDates(long thesisId, long studentId);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class ProgressReportService : IProgressReportService
    {
        private readonly IHttpService _httpService;

        public ProgressReportService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<ProgressReportResponseDTO>> Add(ProgressReportDTO ProgressReport)
        {
            return await _httpService.Post<ResponseWrapper<ProgressReportResponseDTO>>($"ProgressReport/Post", ProgressReport);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"ProgressReport/Delete/{id}");
        }

        public async Task<PaginationModel<ProgressReportResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<ProgressReportResponseDTO>>($"ProgressReport/GetPaginateList", filter);
        }

        public async Task<ResponseWrapper<ProgressReportResponseDTO>> Update(long id, ProgressReportDTO ProgressReport)
        {
            return await _httpService.Put<ResponseWrapper<ProgressReportResponseDTO>>($"ProgressReport/Put/{id}", ProgressReport);
        }

        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"ProgressReport/DownloadFile/{bucketKey}");
        }

        public async Task<ResponseWrapper<ProgressReportResponseDTO>> CalculateStartEndDates(long thesisId, long studentId)
        {
            return await _httpService.Get<ResponseWrapper<ProgressReportResponseDTO>>($"ProgressReport/CalculateStartEndDates?thesisId={thesisId}&studentId={studentId}");
        }

        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"ProgressReport/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}

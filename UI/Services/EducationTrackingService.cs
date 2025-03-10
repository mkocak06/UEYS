using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using Shared.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UI.Helper;
using Shared.ResponseModels.ProtocolProgram;

namespace UI.Services
{
    public interface IEducationTrackingService
    {
        Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetListByStudentId(long studentId);
        Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetForEReport(long studentId);
        Task<PaginationModel<EducationTrackingResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> GetById(long id);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> GetEducationStartByStudentId(long studentId);
        Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetTimeIncreasingRecordsByDate(OpinionFormRequestDTO opinionForm);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> ReturnToMainProgramInProtocol(long studentDependentProgramId, StudentDependentProgramPaginateDTO dependentProgram);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> SendStudentToDependentProgram(long studentDependentProgramId, StudentDependentProgramPaginateDTO dependentProgram);
        Task<ResponseWrapper<int>> GetRemainingDaysForDependentProgram(StudentDependentProgramPaginateDTO dependentProgram);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> Add(EducationTrackingDTO educationTracking);
        Task<ResponseWrapper<EducationTrackingResponseDTO>> Update(long id, EducationTrackingDTO educationTracking);
        Task Delete(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class EducationTrackingService : IEducationTrackingService
    {
        private readonly IHttpService _httpService;

        public EducationTrackingService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> Add(EducationTrackingDTO educationTracking)
        {
            return await _httpService.Post<ResponseWrapper<EducationTrackingResponseDTO>>($"EducationTracking/Post", educationTracking);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"EducationTracking/Delete/{id}");
        }

        public async Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetListByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<EducationTrackingResponseDTO>>>($"EducationTracking/GetListByStudentId/{studentId}");
        }

        public async Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetForEReport(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<EducationTrackingResponseDTO>>>($"EReport/GetEducationTracking/{studentId}");
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<EducationTrackingResponseDTO>>($"EducationTracking/GetById/{id}");
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> ReturnToMainProgramInProtocol(long studentDependentProgramId, StudentDependentProgramPaginateDTO studentDependentProgram)
        {
            return await _httpService.Put<ResponseWrapper<EducationTrackingResponseDTO>>($"EducationTracking/ReturnToMainProgramInProtocol?studentDependentProgramId={studentDependentProgramId}", studentDependentProgram);
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> SendStudentToDependentProgram(long studentDependentProgramId, StudentDependentProgramPaginateDTO studentDependentProgram)
        {
            return await _httpService.Put<ResponseWrapper<EducationTrackingResponseDTO>>($"EducationTracking/SendStudentToDependentProgram?studentId={studentDependentProgramId}", studentDependentProgram);
        }

        public async Task<ResponseWrapper<int>> GetRemainingDaysForDependentProgram(StudentDependentProgramPaginateDTO studentDependentProgram)
        {
            return await _httpService.Post<ResponseWrapper<int>>($"EducationTracking/GetRemainingDaysForDependentProgram", studentDependentProgram);
        }

        public async Task<PaginationModel<EducationTrackingResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<EducationTrackingResponseDTO>>($"EducationTracking/GetPaginateList", filter);
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> Update(long id, EducationTrackingDTO educationTracking)
        {
            return await _httpService.Put<ResponseWrapper<EducationTrackingResponseDTO>>($"EducationTracking/Put/{id}", educationTracking);
        }
        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"EducationTracking/DownloadFile/{bucketKey}");
        }
        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"EducationTracking/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }

        public async Task<ResponseWrapper<EducationTrackingResponseDTO>> GetEducationStartByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<EducationTrackingResponseDTO>>($"EducationTracking/GetEducationStartByStudentId/{studentId}");
        }

        public async Task<ResponseWrapper<List<EducationTrackingResponseDTO>>> GetTimeIncreasingRecordsByDate(OpinionFormRequestDTO opinionForm)
        {
            return await _httpService.Put<ResponseWrapper<List<EducationTrackingResponseDTO>>>("EducationTracking/GetTimeIncreasingRecordsByDate", opinionForm);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace UI.Services
{
    public interface IProtocolProgramService
    {
        Task<ResponseWrapper<List<ProtocolProgramResponseDTO>>> GetAll(ProgramType progType);
        Task<PaginationModel<ProtocolProgramPaginatedResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<PaginationModel<ProtocolProgramPaginatedResponseDTO>> GetArchiveList(FilterDTO filter);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> Add(ProtocolProgramDTO protocolProtocolProgramDTO);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> Get(long id, DocumentTypes docType, ProgramType progType);
        Task<ResponseWrapper<List<ProtocolProgramByUniversityIdResponseDTO>>> GetListByUniversityId(long uniId, ProgramType progType);
        Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetEducatorListForComplementProgram(long programId);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> GetByProgramId(long programId, ProgramType progType);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> Update(long id, ProtocolProgramResponseDTO protocolProtocolProgramDTO);
        Task Delete(long id);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> UnDelete(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }
    public class ProtocolProgramService : IProtocolProgramService
    {
        private readonly IHttpService _httpService;

        public ProtocolProgramService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> Add(ProtocolProgramDTO protocolProtocolProgramDTO)
        {
            return await _httpService.Post<ResponseWrapper<ProtocolProgramResponseDTO>>($"ProtocolProgram/Post", protocolProtocolProgramDTO);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"ProtocolProgram/Delete/{id}");
        }

        public async Task<ResponseWrapper<List<ProtocolProgramResponseDTO>>> GetAll(ProgramType progType)
        {
            return await _httpService.Get<ResponseWrapper<List<ProtocolProgramResponseDTO>>>($"ProtocolProgram/GetList/{progType}");
        }
        public async Task<PaginationModel<ProtocolProgramPaginatedResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<ProtocolProgramPaginatedResponseDTO>>("ProtocolProgram/GetPaginateList", filter);
        }

        public async Task<PaginationModel<ProtocolProgramPaginatedResponseDTO>> GetArchiveList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<ProtocolProgramPaginatedResponseDTO>>("Archive/GetProtocolProgramList", filter);
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> Get(long id, DocumentTypes docType, ProgramType progType)
        {
            return await _httpService.Get<ResponseWrapper<ProtocolProgramResponseDTO>>($"ProtocolProgram/Get?id={id}&docType={docType}&progType={progType}");
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> GetByProgramId(long programId, ProgramType progType)
        {
            return await _httpService.Get<ResponseWrapper<ProtocolProgramResponseDTO>>($"ProtocolProgram/GetByProgramId?programId={programId}&progType={progType}");
        }

        public async Task<ResponseWrapper<List<ProtocolProgramByUniversityIdResponseDTO>>> GetListByUniversityId(long uniId, ProgramType progType)
        {
            return await _httpService.Get<ResponseWrapper<List<ProtocolProgramByUniversityIdResponseDTO>>>($"ProtocolProgram/GetListByUniversityId?uniId={uniId}&progType={progType}");
        }

        public async Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetEducatorListForComplementProgram(long programId)
        {
            return await _httpService.Get<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>>($"ProtocolProgram/GetEducatorListForComplementProgram?programId={programId}");
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> Update(long id, ProtocolProgramResponseDTO protocolProtocolProgramDTO)
        {
            return await _httpService.Put<ResponseWrapper<ProtocolProgramResponseDTO>>($"ProtocolProgram/Put/{id}", protocolProtocolProgramDTO);
        }

        public async Task<ResponseWrapper<ProtocolProgramResponseDTO>> UnDelete(long id)
        {
            return await _httpService.Put<ResponseWrapper<ProtocolProgramResponseDTO>>($"Archive/UnDeleteProtocolProgram/{id}", null);
        }
        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"ProtocolProgram/DownloadFile/{bucketKey}");
        }
        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"ProtocolProgram/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IEducationOfficerService
    {
        Task<PaginationModel<EducationOfficerResponseDTO>> GetPaginateListForProgramDetail(FilterDTO filter);
        Task<ResponseWrapper<EducationOfficerResponseDTO>> Add(EducationOfficerDTO educationOfficer);
        Task<ResponseWrapper<EducationOfficerResponseDTO>> ChangeProgramManager(EducationOfficerDTO educationOfficer);
        Task<ResponseWrapper<List<EducationOfficerResponseDTO>>> GetListByProgramId(long programId);
    }
    public class EducationOfficerService : IEducationOfficerService
    {
        private readonly IHttpService _httpService;

        public EducationOfficerService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<PaginationModel<EducationOfficerResponseDTO>> GetPaginateListForProgramDetail(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<EducationOfficerResponseDTO>>($"EducationOfficer/GetPaginateListForProgramDetail", filter);
        }

        public async Task<ResponseWrapper<EducationOfficerResponseDTO>> Add(EducationOfficerDTO educationOfficer)
        {
            return await _httpService.Post<ResponseWrapper<EducationOfficerResponseDTO>>($"EducationOfficer/Post", educationOfficer);
        }

        public async Task<ResponseWrapper<EducationOfficerResponseDTO>> ChangeProgramManager(EducationOfficerDTO educationOfficer)
        {
            return await _httpService.Post<ResponseWrapper<EducationOfficerResponseDTO>>($"EducationOfficer/ChangeProgramManager", educationOfficer);
        }

        public async Task<ResponseWrapper<List<EducationOfficerResponseDTO>>> GetListByProgramId(long programId)
        {
            return await _httpService.Get<ResponseWrapper<List<EducationOfficerResponseDTO>>>($"EducationOfficer/GetListByProgramId?programId={programId}");
        }
    }
}

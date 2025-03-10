using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UI.Services
{
    public interface IEducatorDependentProgramService
    {
        Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetByIdAsync(long id);
        Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Add(EducatorDependentProgramDTO educatorDependentProgramDTO);
        Task Delete(long educatorId,long dependentProgramId);
        Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Update(long id, EducatorDependentProgramDTO educatorDependentProgramDTO);
    }
    public class EducatorDependentProgramService : IEducatorDependentProgramService
    {
        private readonly IHttpService _httpService;

        public EducatorDependentProgramService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task Delete(long educatorId, long dependentProgramId)
        {
            await _httpService.Delete($"EducatorDependentProgram/Delete?educatorId={educatorId}&dependentProgramId={dependentProgramId}");
        }

        public async Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetByIdAsync(long id)
        {
            return await _httpService.Get<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>>($"EducatorDependentProgram/Get/{id}");
        }
        public async Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Update(long id, EducatorDependentProgramDTO educatorDependentProgramDTO)
        {
            return await _httpService.Put<ResponseWrapper<EducatorDependentProgramResponseDTO>>($"EducatorDependentProgram/Put/{id}", educatorDependentProgramDTO);
        }
        public async Task<ResponseWrapper<EducatorDependentProgramResponseDTO>> Add(EducatorDependentProgramDTO educatorDependentProgramDTO)
        {
            return await _httpService.Post<ResponseWrapper<EducatorDependentProgramResponseDTO>>("EducatorDependentProgram/Post", educatorDependentProgramDTO);
        }
    }
}

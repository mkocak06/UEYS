using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IDependentProgramService
    {
        Task<ResponseWrapper<DependentProgramResponseDTO>> Add(DependentProgramDTO dependentProgram);
        Task<ResponseWrapper<DependentProgramResponseDTO>> Update(long id, DependentProgramDTO dependentProgram);
        Task<bool> Delete(long id);
    }
    public class DependentProgramService : IDependentProgramService
    {
        private readonly IHttpService _httpService;

        public DependentProgramService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<DependentProgramResponseDTO>> Add(DependentProgramDTO dependentProgram)
        {
            return await _httpService.Post<ResponseWrapper<DependentProgramResponseDTO>>($"DependentProgram/Post", dependentProgram);
        }
        public async Task<ResponseWrapper<DependentProgramResponseDTO>> Update(long id, DependentProgramDTO dependentProgram)
        {
            return await _httpService.Put<ResponseWrapper<DependentProgramResponseDTO>>($"DependentProgram/Put/{id}", dependentProgram);
        }
        public async Task<bool> Delete(long id)
        {
            return await _httpService.Delete($"DependentProgram/Delete/{id}");
        }
    }
}

using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UI.Services
{
    public interface IAdvisorThesisService
    {
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> GetByIdAsync(long id);
        Task<ResponseWrapper<List<AdvisorThesisResponseDTO>>> GetListByThesisId(long thesisId);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> Add(AdvisorThesisDTO advisorThesisDTO);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> AddNotEducatorAdvisor(AdvisorThesisDTO advisorThesisDTO);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> Update(long id, AdvisorThesisDTO advisorThesisDTO);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> ChangeCoordinatorAdvisor(long id, ChangeCoordinatorAdvisorThesisDTO advisorThesisDTO);
        Task Delete(long id);
    }
    public class AdvisorThesisService : IAdvisorThesisService
    {
        private readonly IHttpService _httpService;

        public AdvisorThesisService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"AdvisorThesis/Delete/{id}");
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> GetByIdAsync(long id)
        {
            return await _httpService.Get<ResponseWrapper<AdvisorThesisResponseDTO>>($"AdvisorThesis/Get/{id}");
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> Add(AdvisorThesisDTO advisorThesisDTO)
        {
            return await _httpService.Post<ResponseWrapper<AdvisorThesisResponseDTO>>($"AdvisorThesis/Post", advisorThesisDTO);
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> AddNotEducatorAdvisor(AdvisorThesisDTO advisorThesisDTO)
        {
            return await _httpService.Post<ResponseWrapper<AdvisorThesisResponseDTO>>($"AdvisorThesis/AddNotEducatorAdvisor", advisorThesisDTO);
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> Update(long id, AdvisorThesisDTO advisorThesisDTO)
        {
            return await _httpService.Put<ResponseWrapper<AdvisorThesisResponseDTO>>($"AdvisorThesis/Put/{id}", advisorThesisDTO);
        }

        public async Task<ResponseWrapper<AdvisorThesisResponseDTO>> ChangeCoordinatorAdvisor(long id, ChangeCoordinatorAdvisorThesisDTO advisorThesisDTO)
        {
            return await _httpService.Put<ResponseWrapper<AdvisorThesisResponseDTO>>($"AdvisorThesis/ChangeCoordinatorAdvisor/{id}", advisorThesisDTO);
        }

        public async Task<ResponseWrapper<List<AdvisorThesisResponseDTO>>> GetListByThesisId(long thesisId)
        {
            return await _httpService.Get<ResponseWrapper<List<AdvisorThesisResponseDTO>>>($"AdvisorThesis/GetListByThesisId/{thesisId}");
        }
    }
}

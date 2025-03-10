using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;

namespace UI.Services
{
    public interface IStudyService
    {
        Task<ResponseWrapper<List<StudyResponseDTO>>> GetAll();
        Task<PaginationModel<StudyResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<StudyResponseDTO>> GetById(long id);
        Task<ResponseWrapper<StudyResponseDTO>> Add(StudyDTO study);
        Task<ResponseWrapper<StudyResponseDTO>> Update(long id, StudyDTO study);
        Task Delete(long id);

    }
    public class StudyService : IStudyService
    {
        private readonly IHttpService _httpService;

        public StudyService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<List<StudyResponseDTO>>> GetAll()
        {
            return await _httpService.Get<ResponseWrapper<List<StudyResponseDTO>>>("Study/GetList");
        }
        public async Task<PaginationModel<StudyResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<StudyResponseDTO>>($"Study/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<StudyResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<StudyResponseDTO>>($"Study/Get/{id}");
        }
        public async Task<ResponseWrapper<StudyResponseDTO>> Add(StudyDTO study)
        {
            return await _httpService.Post<ResponseWrapper<StudyResponseDTO>>($"Study/Post", study);
        }
        public async Task<ResponseWrapper<StudyResponseDTO>> Update(long id, StudyDTO study)
        {
            return await _httpService.Put<ResponseWrapper<StudyResponseDTO>>($"Study/Put/{id}", study);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"Study/Delete/{id}");
        }
    }
}

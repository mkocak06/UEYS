using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface ISpecificEducationService
    {
        Task<PaginationModel<SpecificEducationResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<SpecificEducationResponseDTO>> GetById(long studentId);
        Task<ResponseWrapper<SpecificEducationResponseDTO>> Update(long id, SpecificEducationDTO specificEducation);
        Task<ResponseWrapper<SpecificEducationResponseDTO>> Add(SpecificEducationDTO specificEducation);
        Task Delete(long id);

    }

    public class SpecificEducationService : ISpecificEducationService
    {
        private readonly IHttpService _httpService;

        public SpecificEducationService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<PaginationModel<SpecificEducationResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<SpecificEducationResponseDTO>>("SpecificEducation/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<SpecificEducationResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<SpecificEducationResponseDTO>>($"SpecificEducation/Get/{id}");
        }
        public async Task<ResponseWrapper<SpecificEducationResponseDTO>> Update(long id, SpecificEducationDTO specificEducation)
        {
            return await _httpService.Put<ResponseWrapper<SpecificEducationResponseDTO>>($"SpecificEducation/Put/{id}", specificEducation);
        }
        public async Task<ResponseWrapper<SpecificEducationResponseDTO>> Add(SpecificEducationDTO specificEducation)
        {
            return await _httpService.Post<ResponseWrapper<SpecificEducationResponseDTO>>($"SpecificEducation/Post", specificEducation);
        }
        public async Task Delete(long id)
        {
            await _httpService.Delete($"SpecificEducation/Delete/{id}");
        }
    }
}


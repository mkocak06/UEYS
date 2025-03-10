using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IStandardService
    {
        Task<PaginationModel<StandardResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<PaginationModel<StandardResponseDTO>> GetPaginateListDistinct(FilterDTO filter);
        Task<ResponseWrapper<StandardResponseDTO>> GetById(long studentId);
        Task<ResponseWrapper<StandardResponseDTO>> Update(long id, StandardDTO standart);
        Task<ResponseWrapper<StandardResponseDTO>> Add(StandardDTO standart);
        Task Delete(long id);

    }

    public class StandardService : IStandardService
    {
        private readonly IHttpService _httpService;

        public StandardService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<PaginationModel<StandardResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<StandardResponseDTO>>("Standard/GetPaginateList", filter);
        }
        public async Task<PaginationModel<StandardResponseDTO>> GetPaginateListDistinct(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<StandardResponseDTO>>("Standard/GetPaginateListDistinct", filter);
        }
        public async Task<ResponseWrapper<StandardResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<StandardResponseDTO>>($"Standard/Get/{id}");
        }
        public async Task<ResponseWrapper<StandardResponseDTO>> Update(long id, StandardDTO standart)
        {
            return await _httpService.Put<ResponseWrapper<StandardResponseDTO>>($"Standard/Put/{id}", standart);
        }
        public async Task<ResponseWrapper<StandardResponseDTO>> Add(StandardDTO standart)
        {
            return await _httpService.Post<ResponseWrapper<StandardResponseDTO>>($"Standard/Post", standart);
        }
        public async Task Delete(long id)
        {
            await _httpService.Delete($"Standard/Delete/{id}");
        }
    }
}


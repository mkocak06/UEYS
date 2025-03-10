using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IStandardCategoryService
    {
        Task<PaginationModel<StandardCategoryResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> GetById(long categoryId);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> Update(long id, StandardCategoryDTO category);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> Add(StandardCategoryDTO category);
        Task Delete(long id);

    }

    public class StandardCategoryService : IStandardCategoryService
    {
        private readonly IHttpService _httpService;

        public StandardCategoryService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<PaginationModel<StandardCategoryResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<StandardCategoryResponseDTO>>("StandardCategory/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<StandardCategoryResponseDTO>> GetById(long categoryId)
        {
            return await _httpService.Get<ResponseWrapper<StandardCategoryResponseDTO>>($"StandardCategory/Get/{categoryId}");
        }
        public async Task<ResponseWrapper<StandardCategoryResponseDTO>> Update(long id, StandardCategoryDTO category)
        {
            return await _httpService.Put<ResponseWrapper<StandardCategoryResponseDTO>>($"StandardCategory/Put/{id}", category);
        }
        public async Task<ResponseWrapper<StandardCategoryResponseDTO>> Add(StandardCategoryDTO category)
        {
            return await _httpService.Post<ResponseWrapper<StandardCategoryResponseDTO>>($"StandardCategory/Post", category);
        }
        public async Task Delete(long id)
        {
            await _httpService.Delete($"StandardCategory/Delete/{id}");
        }

    }
}


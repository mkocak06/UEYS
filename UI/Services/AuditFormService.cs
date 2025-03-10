using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IAuditFormService
    {
        Task<PaginationModel<StandardCategoryResponseDTO>> GetStandardCategoryList(FilterDTO filter);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> GetStandardCategoryById(long categoryId);
        Task<ResponseWrapper<StandardCategoryResponseDTO>> UpdateStandardCategory(long id, StandardCategoryDTO category);
        Task DeleteStandardCategory(long id);

        Task<PaginationModel<StandardResponseDTO>> GetStandardList(FilterDTO filter);
        Task<ResponseWrapper<StandardResponseDTO>> GetStandardById(long studentId);
        Task<ResponseWrapper<StandardResponseDTO>> UpdateStandard(long id, StandardDTO standart);

        Task<ResponseWrapper<FormResponseDTO>> AddForm(FormDTO form);
        Task<ResponseWrapper<FormResponseDTO>> UpdateForm(long id, FormDTO form);
        Task<PaginationModel<FormResponseDTO>> GetFormPaginateList(FilterDTO filter);
        Task<ResponseWrapper<FormResponseDTO>> GetFormById(long formId);
        Task DeleteStandard(long id);

    }

    public class AuditFormService : IAuditFormService
    {
        private readonly IHttpService _httpService;

        public AuditFormService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        #region Form Standard Category
        public async Task<PaginationModel<StandardCategoryResponseDTO>> GetStandardCategoryList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<StandardCategoryResponseDTO>>("StandardCategory/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<StandardCategoryResponseDTO>> GetStandardCategoryById(long categoryId)
        {
            return await _httpService.Get<ResponseWrapper<StandardCategoryResponseDTO>>($"StandardCategory/Get/{categoryId}");
        }
        public async Task<ResponseWrapper<StandardCategoryResponseDTO>> UpdateStandardCategory(long id, StandardCategoryDTO category)
        {
            return await _httpService.Put<ResponseWrapper<StandardCategoryResponseDTO>>($"StandardCategory/Put/{id}", category);
        }
        public async Task DeleteStandardCategory(long id)
        {
            await _httpService.Delete($"StandardCategory/Delete/{id}");
        }
        #endregion

        #region Form Standard
        public async Task<PaginationModel<StandardResponseDTO>> GetStandardList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<StandardResponseDTO>>("Standard/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<StandardResponseDTO>> GetStandardById(long id)
        {
            return await _httpService.Get<ResponseWrapper<StandardResponseDTO>>($"Standard/Get/{id}");
        }
        public async Task<ResponseWrapper<StandardResponseDTO>> UpdateStandard(long id, StandardDTO standart)
        {
            return await _httpService.Put<ResponseWrapper<StandardResponseDTO>>($"Standard/Put/{id}", standart);
        }
        public async Task DeleteStandard(long id)
        {
            await _httpService.Delete($"Standard/Delete/{id}");
        }

        public async Task<ResponseWrapper<FormResponseDTO>> AddForm(FormDTO form)
        {
           return await _httpService.Post<ResponseWrapper<FormResponseDTO>>("Form/Post", form);
        }
        public async Task<ResponseWrapper<FormResponseDTO>> GetFormById(long formId)
        {
            return await _httpService.Get<ResponseWrapper<FormResponseDTO>>($"Form/Get/{formId}");
        }
        public async Task<PaginationModel<FormResponseDTO>> GetFormPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<FormResponseDTO>>("Form/GetPaginateList", filter);
        }

        public async Task<ResponseWrapper<FormResponseDTO>> UpdateForm(long id, FormDTO form)
        {
            return await _httpService.Put<ResponseWrapper<FormResponseDTO>>($"Form/Put/{id}", form);
        }
        #endregion
    }
}


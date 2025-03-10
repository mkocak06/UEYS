using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IAuthorizationCategoryService
{
    Task<ResponseWrapper<List<AuthorizationCategoryResponseDTO>>> GetAll();
    Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> GetById(long id);
    Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Add(AuthorizationCategoryDTO authorizationCategory);
    Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Update(long id, AuthorizationCategoryDTO authorizationCategory);
    Task<PaginationModel<AuthorizationCategoryResponseDTO>> GetPaginateList(FilterDTO filter);
    Task Delete(long id);
    Task<ResponseWrapper<bool>> ChangeOrder(long id, bool isToUp);
}

public class AuthorizationCategoryService : IAuthorizationCategoryService
{
    private readonly IHttpService _httpService;

    public AuthorizationCategoryService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<ResponseWrapper<List<AuthorizationCategoryResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<AuthorizationCategoryResponseDTO>>>("AuthorizationCategory/GetList");
    }

    public async Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<AuthorizationCategoryResponseDTO>>($"AuthorizationCategory/Get/{id}");
    }

    public async Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Add(AuthorizationCategoryDTO authorizationCategory)
    {
        return await _httpService.Post<ResponseWrapper<AuthorizationCategoryResponseDTO>>($"AuthorizationCategory/Post", authorizationCategory);
    }

    public async Task<ResponseWrapper<AuthorizationCategoryResponseDTO>> Update(long id, AuthorizationCategoryDTO authorizationCategory)
    {
        return await _httpService.Put<ResponseWrapper<AuthorizationCategoryResponseDTO>>($"AuthorizationCategory/Put/{id}", authorizationCategory);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"AuthorizationCategory/Delete/{id}");
    }

    public async Task<PaginationModel<AuthorizationCategoryResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<AuthorizationCategoryResponseDTO>>($"AuthorizationCategory/GetPaginateList", filter);
    }

    public async Task<ResponseWrapper<bool>> ChangeOrder(long id, bool isToUp)
    {
        return await _httpService.Get<ResponseWrapper<bool>>($"AuthorizationCategory/ChangeOrder?id={id}&isToUp={isToUp}");
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.FilterModels.Base;
using Shared.Types;

namespace UI.Services;

public interface ITitleService
{
    Task<ResponseWrapper<List<TitleResponseDTO>>> GetAll();
    Task<ResponseWrapper<List<TitleResponseDTO>>> GetListByType(TitleType titleType);
    Task<ResponseWrapper<TitleResponseDTO>> GetById(long id);
    Task<PaginationModel<TitleResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<TitleResponseDTO>> Add(TitleDTO title);
    Task<ResponseWrapper<TitleResponseDTO>> Update(long id, TitleDTO title);
    Task Delete(long id);
}

public class TitleService : ITitleService
{
    private readonly IHttpService _httpService;

    public TitleService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<TitleResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<TitleResponseDTO>>>("Title/GetList");
    }
    public async Task<PaginationModel<TitleResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<TitleResponseDTO>>($"Title/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<TitleResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<TitleResponseDTO>>($"Title/Get/{id}");
    }

    public async Task<ResponseWrapper<TitleResponseDTO>> Add(TitleDTO title)
    {
        return await _httpService.Post<ResponseWrapper<TitleResponseDTO>>($"Title/Post", title);
    }

    public async Task<ResponseWrapper<TitleResponseDTO>> Update(long id, TitleDTO title)
    {
        return await _httpService.Put<ResponseWrapper<TitleResponseDTO>>($"Title/Put/{id}", title);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Title/Delete/{id}");
    }

    public async Task<ResponseWrapper<List<TitleResponseDTO>>> GetListByType(TitleType titleType)
    {
        return await _httpService.Get<ResponseWrapper<List<TitleResponseDTO>>>($"Title/GetListByType/{titleType}");
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.FilterModels.Base;
using Shared.Types;

namespace UI.Services;

public interface ISpecificEducationPlaceService
{
    Task<ResponseWrapper<List<SpecificEducationPlaceResponseDTO>>> GetAll();
    Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> GetById(long id);
    Task<PaginationModel<SpecificEducationPlaceResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Add(SpecificEducationPlaceDTO specificEducationPlace);
    Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Update(long id, SpecificEducationPlaceDTO specificEducationPlace);
    Task Delete(long id);
}

public class SpecificEducationPlaceService : ISpecificEducationPlaceService
{
    private readonly IHttpService _httpService;

    public SpecificEducationPlaceService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<SpecificEducationPlaceResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<SpecificEducationPlaceResponseDTO>>>("SpecificEducationPlace/GetList");
    }
    public async Task<PaginationModel<SpecificEducationPlaceResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<SpecificEducationPlaceResponseDTO>>($"SpecificEducationPlace/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<SpecificEducationPlaceResponseDTO>>($"SpecificEducationPlace/Get/{id}");
    }

    public async Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Add(SpecificEducationPlaceDTO specificEducationPlace)
    {
        return await _httpService.Post<ResponseWrapper<SpecificEducationPlaceResponseDTO>>($"SpecificEducationPlace/Post", specificEducationPlace);
    }

    public async Task<ResponseWrapper<SpecificEducationPlaceResponseDTO>> Update(long id, SpecificEducationPlaceDTO specificEducationPlace)
    {
        return await _httpService.Put<ResponseWrapper<SpecificEducationPlaceResponseDTO>>($"SpecificEducationPlace/Put/{id}", specificEducationPlace);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"SpecificEducationPlace/Delete/{id}");
    }
}
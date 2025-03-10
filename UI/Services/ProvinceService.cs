using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IProvinceService
{
    Task<ResponseWrapper<List<ProvinceResponseDTO>>> GetAll();
    Task<ResponseWrapper<List<CityDetailsForMapModel>>> GetListForMap();
    Task<ResponseWrapper<ProvinceResponseDTO>> GetById(long id);
    Task<PaginationModel<ProvinceResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<ProvinceResponseDTO>> Add(ProvinceDTO province);
    Task<ResponseWrapper<ProvinceResponseDTO>> Update(long id, ProvinceDTO province);
    Task Delete(long id);
}

public class ProvinceService : IProvinceService
{
    private readonly IHttpService _httpService;

    public ProvinceService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<PaginationModel<ProvinceResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProvinceResponseDTO>>($"Province/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<List<ProvinceResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<ProvinceResponseDTO>>>("Province/GetList");
    }
    public async Task<ResponseWrapper<List<CityDetailsForMapModel>>> GetListForMap()
    {
        return await _httpService.Get<ResponseWrapper<List<CityDetailsForMapModel>>>("Province/GetProvinceDetailForMap");
    }

    public async Task<ResponseWrapper<ProvinceResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<ProvinceResponseDTO>>($"Province/Get/{id}");
    }

    public async Task<ResponseWrapper<ProvinceResponseDTO>> Add(ProvinceDTO province)
    {
        return await _httpService.Post<ResponseWrapper<ProvinceResponseDTO>>($"Province/Post", province);
    }

    public async Task<ResponseWrapper<ProvinceResponseDTO>> Update(long id, ProvinceDTO province)
    {
        return await _httpService.Put<ResponseWrapper<ProvinceResponseDTO>>($"Province/Put/{id}", province);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Province/Delete/{id}");
    }
}
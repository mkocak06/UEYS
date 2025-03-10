using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IPortfolioService
{
    Task<ResponseWrapper<PortfolioResponseDTO>> GetById(long id);
    Task<PaginationModel<PortfolioResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<PortfolioResponseDTO>> Add(PortfolioDTO portfolio);
    Task<ResponseWrapper<PortfolioResponseDTO>> Update(long id, PortfolioDTO portfolio);
    Task Delete(long id);
}

public class PortfolioService : IPortfolioService
{
    private readonly IHttpService _httpService;

    public PortfolioService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<PaginationModel<PortfolioResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<PortfolioResponseDTO>>($"Portfolio/GetPaginateList", filter);
    }

    public async Task<ResponseWrapper<PortfolioResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<PortfolioResponseDTO>>($"Portfolio/Get/{id}");
    }

    public async Task<ResponseWrapper<PortfolioResponseDTO>> Add(PortfolioDTO portfolio)
    {
        return await _httpService.Post<ResponseWrapper<PortfolioResponseDTO>>($"Portfolio/Post", portfolio);
    }

    public async Task<ResponseWrapper<PortfolioResponseDTO>> Update(long id, PortfolioDTO portfolio)
    {
        return await _httpService.Put<ResponseWrapper<PortfolioResponseDTO>>($"Portfolio/Put/{id}", portfolio);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Portfolio/Delete/{id}");
    }
}
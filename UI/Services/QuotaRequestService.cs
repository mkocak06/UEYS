using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IQuotaRequestService
{
    Task<ResponseWrapper<QuotaRequestResponseDTO>> GetById(long id);
    Task<PaginationModel<QuotaRequestResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<QuotaRequestResponseDTO>> Add(QuotaRequestDTO quotaRequest);
    Task<ResponseWrapper<QuotaRequestResponseDTO>> Update(long id, QuotaRequestDTO quotaRequest);
    Task Delete(long id);
}

public class QuotaRequestService : IQuotaRequestService
{
    private readonly IHttpService _httpService;

    public QuotaRequestService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<PaginationModel<QuotaRequestResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<QuotaRequestResponseDTO>>($"QuotaRequest/GetPaginateList", filter);
    }

    public async Task<ResponseWrapper<QuotaRequestResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<QuotaRequestResponseDTO>>($"QuotaRequest/Get/{id}");
    }

    public async Task<ResponseWrapper<QuotaRequestResponseDTO>> Add(QuotaRequestDTO quotaRequest)
    {
        return await _httpService.Post<ResponseWrapper<QuotaRequestResponseDTO>>($"QuotaRequest/Post", quotaRequest);
    }

    public async Task<ResponseWrapper<QuotaRequestResponseDTO>> Update(long id, QuotaRequestDTO quotaRequest)
    {
        return await _httpService.Put<ResponseWrapper<QuotaRequestResponseDTO>>($"QuotaRequest/Put/{id}", quotaRequest);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"QuotaRequest/Delete/{id}");
    }
}
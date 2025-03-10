using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface ISubQuotaRequestService
{
    Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetById(long id);
    Task<PaginationModel<SubQuotaRequestPaginateResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Add(SubQuotaRequestDTO subQuotaRequest);
    Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Update(long id, SubQuotaRequestDTO subQuotaRequest);
    Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetByProgramId(long programId);
    Task<ResponseWrapper<byte[]>> ExcelExport(FilterDTO filter);
    Task Delete(long id);
}

public class SubQuotaRequestService : ISubQuotaRequestService
{
    private readonly IHttpService _httpService;

    public SubQuotaRequestService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<PaginationModel<SubQuotaRequestPaginateResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<SubQuotaRequestPaginateResponseDTO>>($"SubQuotaRequest/GetPaginateList", filter);
    }

    public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<SubQuotaRequestResponseDTO>>($"SubQuotaRequest/Get/{id}");
    }
    
    public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetByProgramId(long programId)
    {
        return await _httpService.Get<ResponseWrapper<SubQuotaRequestResponseDTO>>($"SubQuotaRequest/GetByProgramId/{programId}");
    }

    public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Add(SubQuotaRequestDTO subQuotaRequest)
    {
        return await _httpService.Post<ResponseWrapper<SubQuotaRequestResponseDTO>>($"SubQuotaRequest/Post", subQuotaRequest);
    }

    public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Update(long id, SubQuotaRequestDTO subQuotaRequest)
    {
        return await _httpService.Put<ResponseWrapper<SubQuotaRequestResponseDTO>>($"SubQuotaRequest/Put/{id}", subQuotaRequest);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"SubQuotaRequest/Delete/{id}");
    }
    public async Task<ResponseWrapper<byte[]>> ExcelExport(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<byte[]>>("SubQuotaRequest/ExcelExport", filter);
    }
}
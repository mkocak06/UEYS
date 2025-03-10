using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.FilterModels.Base;
using Shared.Types;

namespace UI.Services;

public interface ILogService
{
    Task<ResponseWrapper<List<LogResponseDTO>>> GetAll();
    Task<ResponseWrapper<List<LogResponseDTO>>> GetListByType(LogType LogType);
    Task<ResponseWrapper<LogResponseDTO>> GetById(long id);
    Task<PaginationModel<LogResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<LogResponseDTO>> Add(LogDTO Log);
    Task<ResponseWrapper<LogResponseDTO>> Update(long id, LogDTO Log);
    Task Delete(long id);
}

public class LogService : ILogService
{
    private readonly IHttpService _httpService;

    public LogService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<LogResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<LogResponseDTO>>>("Log/GetList");
    }
    public async Task<PaginationModel<LogResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<LogResponseDTO>>($"Log/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<LogResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<LogResponseDTO>>($"Log/Get/{id}");
    }

    public async Task<ResponseWrapper<LogResponseDTO>> Add(LogDTO Log)
    {
        return await _httpService.Post<ResponseWrapper<LogResponseDTO>>($"Log/Post", Log);
    }

    public async Task<ResponseWrapper<LogResponseDTO>> Update(long id, LogDTO Log)
    {
        return await _httpService.Put<ResponseWrapper<LogResponseDTO>>($"Log/Put/{id}", Log);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Log/Delete/{id}");
    }

    public async Task<ResponseWrapper<List<LogResponseDTO>>> GetListByType(LogType LogType)
    {
        return await _httpService.Get<ResponseWrapper<List<LogResponseDTO>>>($"Log/GetListByType/{LogType}");
    }
}
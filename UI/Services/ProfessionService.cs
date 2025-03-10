using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IProfessionService
{
    Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetAll();
    Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetAllByUniversityId(long id);
    Task<ResponseWrapper<ProfessionResponseDTO>> GetById(long id);
    Task<ResponseWrapper<ProfessionResponseDTO>> Add(ProfessionDTO faculty);
    Task<ResponseWrapper<ProfessionResponseDTO>> Update(long id, ProfessionDTO faculty);
    Task<PaginationModel<ProfessionResponseDTO>> GetPaginateList(FilterDTO filter);
    Task Delete(long id);
}

public class ProfessionService : IProfessionService
{
    private readonly IHttpService _httpService;

    public ProfessionService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<ProfessionResponseDTO>>>("Profession/GetList");
    }

    public async Task<ResponseWrapper<List<ProfessionResponseDTO>>> GetAllByUniversityId(long id)
    {
        return await _httpService.Get<ResponseWrapper<List<ProfessionResponseDTO>>>($"Profession/GetByUniversityId/{id}");
    }

    public async Task<ResponseWrapper<ProfessionResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<ProfessionResponseDTO>>($"Profession/Get/{id}");
    }

    public async Task<ResponseWrapper<ProfessionResponseDTO>> Add(ProfessionDTO faculty)
    {
        return await _httpService.Post<ResponseWrapper<ProfessionResponseDTO>>($"Profession/Post", faculty);
    }

    public async Task<ResponseWrapper<ProfessionResponseDTO>> Update(long id, ProfessionDTO faculty)
    {
        return await _httpService.Put<ResponseWrapper<ProfessionResponseDTO>>($"Profession/Put/{id}", faculty);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Profession/Delete/{id}");
    }

    public async Task<PaginationModel<ProfessionResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProfessionResponseDTO>>($"Profession/GetPaginateList", filter);
    }
}
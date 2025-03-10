using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Threading.Tasks;

namespace UI.Services;

public interface ICurriculumPerfectionService
{
    Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> GetByIdAsync(long id);
    Task<PaginationModel<CurriculumPerfectionResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<CurriculumPerfectionResponseDTO>> GetArchiveList(FilterDTO filter);
    Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> PostAsync(CurriculumPerfectionDTO perfection);
    Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Put(long id, CurriculumPerfectionDTO curriculumPerfection);
    Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Undelete(long id);
    Task Delete(long id);

}

public class CurriculumPerfectionService : ICurriculumPerfectionService
{
    private readonly IHttpService _httpService;

    public CurriculumPerfectionService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> GetByIdAsync(long id)
    {
        return await _httpService.Get<ResponseWrapper<CurriculumPerfectionResponseDTO>>($"CurriculumPerfection/Get/{id}");
    }

    public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> PostAsync(CurriculumPerfectionDTO perfection)
    {
        return await _httpService.Post<ResponseWrapper<CurriculumPerfectionResponseDTO>>($"CurriculumPerfection/Post", perfection);
    }

    public async Task<PaginationModel<CurriculumPerfectionResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<CurriculumPerfectionResponseDTO>>("CurriculumPerfection/GetPaginateList", filter);
    }

    public async Task<PaginationModel<CurriculumPerfectionResponseDTO>> GetArchiveList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<CurriculumPerfectionResponseDTO>>("Archive/GetCurriculumPerfectionList", filter);
    }

    public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Put(long id, CurriculumPerfectionDTO curriculumPerfection)
    {
        return await _httpService.Put<ResponseWrapper<CurriculumPerfectionResponseDTO>>($"CurriculumPerfection/Put/{id}", curriculumPerfection);
    }

    public async Task<ResponseWrapper<CurriculumPerfectionResponseDTO>> Undelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<CurriculumPerfectionResponseDTO>>($"Archive/UnDeleteCurriculumPerfection/{id}", null);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"CurriculumPerfection/Delete/{id}");
    }
}
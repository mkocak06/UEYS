using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Threading.Tasks;

namespace UI.Services;

public interface ICurriculumRotationService
{
    Task<ResponseWrapper<CurriculumRotationResponseDTO>> GetByIdAsync(long id);
    Task<PaginationModel<CurriculumRotationResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<CurriculumRotationResponseDTO>> GetArchiveList(FilterDTO filter);
    Task<ResponseWrapper<CurriculumRotationResponseDTO>> PostAsync(CurriculumRotationDTO Rotation);
    Task<ResponseWrapper<CurriculumRotationResponseDTO>> Put(long id, CurriculumRotationDTO curriculumRotation);
    Task<ResponseWrapper<CurriculumRotationResponseDTO>> UnDelete(long id);
    Task Delete(long id);

}

public class CurriculumRotationService : ICurriculumRotationService
{
    private readonly IHttpService _httpService;

    public CurriculumRotationService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> GetByIdAsync(long id)
    {
        return await _httpService.Get<ResponseWrapper<CurriculumRotationResponseDTO>>($"CurriculumRotation/Get/{id}");
    }

    public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> PostAsync(CurriculumRotationDTO Rotation)
    {
        return await _httpService.Post<ResponseWrapper<CurriculumRotationResponseDTO>>($"CurriculumRotation/Post", Rotation);
    }

    public async Task<PaginationModel<CurriculumRotationResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<CurriculumRotationResponseDTO>>("CurriculumRotation/GetPaginateList", filter);
    }

    public async Task<PaginationModel<CurriculumRotationResponseDTO>> GetArchiveList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<CurriculumRotationResponseDTO>>("Archive/GetCurriculumRotationList", filter);
    }

    public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> Put(long id, CurriculumRotationDTO curriculumRotation)
    {
        return await _httpService.Put<ResponseWrapper<CurriculumRotationResponseDTO>>($"CurriculumRotation/Put/{id}", curriculumRotation);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"CurriculumRotation/Delete/{id}");
    }

    public async Task<ResponseWrapper<CurriculumRotationResponseDTO>> UnDelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<CurriculumRotationResponseDTO>>($"Archive/UnDeleteCurriculumRotation/{id}", null);
    }
}

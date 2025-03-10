using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using UI.Helper;

namespace UI.Services;

public interface IRotationService
{
    Task<PaginationModel<RotationResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<RotationResponseDTO>> GetByIdAsync(long id);
    Task<ResponseWrapper<RotationResponseDTO>> PostAsync(RotationDTO rotationDTO);
    Task<ResponseWrapper<RotationResponseDTO>> Put(long id, RotationDTO rotationDTO);
    Task<ResponseWrapper<List<RotationResponseDTO>>> GetListByCurriculumId(long curriculumId);
    Task<ResponseWrapper<List<CurriculumRotationResponseDTO>>> GetListByStudentId(long studentId);
    Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerStudentsListByUserId(long userId);
    Task Delete(long id);
}

public class RotationService : IRotationService
{
    private readonly IHttpService _httpService;

    public RotationService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<PaginationModel<RotationResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<RotationResponseDTO>>("Rotation/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<List<CurriculumRotationResponseDTO>>> GetListByStudentId(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<List<CurriculumRotationResponseDTO>>>($"Rotation/GetListByStudentId/{studentId}");
    }
    public async Task<ResponseWrapper<List<StudentRotationResponseDTO>>> GetFormerStudentsListByUserId(long userId)
    {
        return await _httpService.Get<ResponseWrapper<List<StudentRotationResponseDTO>>>($"StudentRotation/GetFormerStudentsListByUserId/{userId}");
    }
    public async Task<ResponseWrapper<List<RotationResponseDTO>>> GetListByCurriculumId(long curriculumId)
    {
        return await _httpService.Get<ResponseWrapper<List<RotationResponseDTO>>>($"Rotation/GetListByCurriculumId/{curriculumId}");
    }
    public async Task<ResponseWrapper<RotationResponseDTO>> GetByIdAsync(long id)
    {
        return await _httpService.Get<ResponseWrapper<RotationResponseDTO>>($"Rotation/Get/{id}");
    }

    public async Task<ResponseWrapper<RotationResponseDTO>> PostAsync(RotationDTO rotation)
    {
        return await _httpService.Post<ResponseWrapper<RotationResponseDTO>>($"Rotation/Post", rotation);
    }

    public async Task<ResponseWrapper<RotationResponseDTO>> Put(long id, RotationDTO rotation)
    {
        return await _httpService.Put<ResponseWrapper<RotationResponseDTO>>($"Rotation/Put/{id}", rotation);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Rotation/Delete/{id}");
    }
}
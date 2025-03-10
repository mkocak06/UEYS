using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.FilterModels.Base;
using Shared.Types;
using System.Threading;

namespace UI.Services;

public interface IStudentRotationPerfectionService
{
    Task<ResponseWrapper<List<StudentRotationPerfectionResponseDTO>>> GetAll();
    Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> GetByStrIdAndPerfectionId(long studentRotationId, long perfectionId);
    Task<PaginationModel<StudentRotationPerfectionResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Add(StudentRotationPerfectionDTO StudentRotationPerfection);
    Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Update(long id, StudentRotationPerfectionDTO StudentRotationPerfection);
    Task Delete(long id);
}

public class StudentRotationPerfectionService : IStudentRotationPerfectionService
{
    private readonly IHttpService _httpService;

    public StudentRotationPerfectionService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<StudentRotationPerfectionResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<StudentRotationPerfectionResponseDTO>>>("StudentRotationPerfection/GetList");
    }
    public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> GetByStrIdAndPerfectionId(long studentRotationId, long perfectionId)
    {
        return await _httpService.Get<ResponseWrapper<StudentRotationPerfectionResponseDTO>>($"StudentRotationPerfection/GetByStrIdAndPerfectionId?studentRotationId={studentRotationId}&perfectionId={perfectionId}");
    }
    public async Task<PaginationModel<StudentRotationPerfectionResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<StudentRotationPerfectionResponseDTO>>($"StudentRotationPerfection/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<StudentRotationPerfectionResponseDTO>>($"StudentRotationPerfection/Get/{id}");
    }

    public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Add(StudentRotationPerfectionDTO StudentRotationPerfection)
    {
        return await _httpService.Post<ResponseWrapper<StudentRotationPerfectionResponseDTO>>("StudentRotationPerfection/Post", StudentRotationPerfection);
    }

    public async Task<ResponseWrapper<StudentRotationPerfectionResponseDTO>> Update(long id, StudentRotationPerfectionDTO StudentRotationPerfection)
    {
        return await _httpService.Put<ResponseWrapper<StudentRotationPerfectionResponseDTO>>($"StudentRotationPerfection/Put/{id}", StudentRotationPerfection);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"StudentRotationPerfection/Delete/{id}");
    }

}
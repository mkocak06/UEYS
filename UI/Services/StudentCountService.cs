using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IStudentCountService
{
    Task<PaginationModel<StudentCountResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<StudentCountResponseDTO>> GetById(long id);
    Task<ResponseWrapper<StudentCountResponseDTO>> Add(StudentCountDTO studentCount);
    Task<ResponseWrapper<StudentCountResponseDTO>> Update(long id, StudentCountDTO studentCount);
    Task<ResponseWrapper<StudentCountResponseDTO>> GetByProgramId(long programId);
    Task Delete(long id);
}

public class StudentCountService : IStudentCountService
{
    private readonly IHttpService _httpService;
    public StudentCountService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<PaginationModel<StudentCountResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<StudentCountResponseDTO>>("StudentCount/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<StudentCountResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<StudentCountResponseDTO>>($"StudentCount/Get/{id}");
    }

    public async Task<ResponseWrapper<StudentCountResponseDTO>> GetByProgramId(long programId)
    {
        return await _httpService.Get<ResponseWrapper<StudentCountResponseDTO>>($"StudentCount/GetByProgramId/{programId}");
    }

    public async Task<ResponseWrapper<StudentCountResponseDTO>> Add(StudentCountDTO studentCount)
    {
        return await _httpService.Post<ResponseWrapper<StudentCountResponseDTO>>($"StudentCount/Post", studentCount);
    }

    public async Task<ResponseWrapper<StudentCountResponseDTO>> Update(long id, StudentCountDTO studentCount)
    {
        return await _httpService.Put<ResponseWrapper<StudentCountResponseDTO>>($"StudentCount/Put/{id}", studentCount);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"StudentCount/Delete/{id}");
    }
}
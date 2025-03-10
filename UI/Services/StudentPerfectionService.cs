using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace UI.Services;

public interface IStudentPerfectionService
{
    Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByIdAsync( long id);
    Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByStudentAndPerfectionIdAsync(long studentId, long perfectionId);
    Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetListByStudentId(long studentId, PerfectionType perfectionType);
    Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetForEReport(long studentId, PerfectionType perfectionType);
    Task<ResponseWrapper<StudentPerfectionResponseDTO>> PostAsync( StudentPerfectionDTO studentPerfectionDTO);
    Task<ResponseWrapper<StudentPerfectionResponseDTO>> PutAsync(long id, StudentPerfectionDTO StudentPerfection);
    Task Delete(long studentId, long perfectionId);
    Task<ResponseWrapper<StudentPerfectionResponseDTO>> CompleteAllStudentPerfections(long studentId, PerfectionType perfectionType);
}

public class StudentPerfectionService : IStudentPerfectionService
{
    private readonly IHttpService _httpService;

    public StudentPerfectionService(IHttpService httpService)
    {
        _httpService = httpService;
    }
   
    public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByIdAsync(long id)
    {
        return await _httpService.Get<ResponseWrapper<StudentPerfectionResponseDTO>>($"StudentPerfection/GetAsync/{id}");
    }

    public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> GetByStudentAndPerfectionIdAsync(long studentId, long perfectionId)
    {
        return await _httpService.Get<ResponseWrapper<StudentPerfectionResponseDTO>>($"StudentPerfection/GetByStudentAndPerfectionId?studentId={studentId}&perfectionId={perfectionId}");
    }

    public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> PostAsync(StudentPerfectionDTO Student)
    {
        return await _httpService.Post<ResponseWrapper<StudentPerfectionResponseDTO>>($"StudentPerfection/Post", Student);
    }

    public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> PutAsync(long id, StudentPerfectionDTO StudentPerfection)
    {
        return await _httpService.Put<ResponseWrapper<StudentPerfectionResponseDTO>>($"StudentPerfection/Put/{id}", StudentPerfection);
    }

    public async Task Delete(long studentId, long perfectionId)
    {
        await _httpService.Delete($"StudentPerfection/Delete?studentId={studentId}&perfectionId={perfectionId}");
    }

    public async Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetListByStudentId(long studentId, PerfectionType perfectionType)
    {
        return await _httpService.Get<ResponseWrapper<List<StudentPerfectionResponseDTO>>>($"StudentPerfection/GetByStudentId?studentId={studentId}&perfectionType={perfectionType}");
    }

    public async Task<ResponseWrapper<List<StudentPerfectionResponseDTO>>> GetForEReport(long studentId, PerfectionType perfectionType)
    {
        return await _httpService.Get<ResponseWrapper<List<StudentPerfectionResponseDTO>>>($"EReport/GetPerfection?studentId={studentId}&perfectionType={perfectionType}");
    }

    public async Task<ResponseWrapper<StudentPerfectionResponseDTO>> CompleteAllStudentPerfections(long studentId, PerfectionType perfectionType)
    {
        return await _httpService.Post<ResponseWrapper<StudentPerfectionResponseDTO>>($"StudentPerfection/CompleteAllPerfections?studentId={studentId}&perfectionType={perfectionType}", null);
    }
}
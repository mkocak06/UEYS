using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;

namespace UI.Services;

public interface IPerfectionService
{
    Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListAsync();
    Task<ResponseWrapper<PerfectionResponseDTO>> GetByIdAsync(long id);
    Task<PaginationModel<PerfectionResponseDTO>> GetPaginateList(FilterDTO filter);
     Task<ResponseWrapper<PerfectionResponseDTO>> PostAsync(PerfectionDTO perfection);
    Task<ResponseWrapper<PerfectionResponseDTO>> PostRotationPerfection(long curriculumId, long rotationId, PerfectionDTO perfection);
    Task<ResponseWrapper<PerfectionResponseDTO>> Put(long id, PerfectionDTO perfection);
    Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListByStudentId(long studentId, PerfectionType perfectionType);
    Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListByCurriculumId(long curriculumId);
    Task Delete(long id);
    Task<ResponseWrapper<byte[]>> GetExcelByteArrayClinical(long studentId);
    Task<ResponseWrapper<byte[]>> GetExcelByteArrayInterventional(long studentId);
}

public class PerfectionService : IPerfectionService
{
    private readonly IHttpService _httpService;

    public PerfectionService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListAsync()
    {
        return await _httpService.Get<ResponseWrapper<List<PerfectionResponseDTO>>>("Perfection/GetList");
    }

    public async Task<ResponseWrapper<PerfectionResponseDTO>> GetByIdAsync(long id)
    {
        return await _httpService.Get<ResponseWrapper<PerfectionResponseDTO>>($"Perfection/Get/{id}");
    }

    public async Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListByStudentId(long studentId, PerfectionType perfectionType)
    {
        return await _httpService.Get<ResponseWrapper<List<PerfectionResponseDTO>>>($"Perfection/GetListByStudentId?studentId={studentId}&perfectionType={perfectionType}");
    }

    public async Task<ResponseWrapper<List<PerfectionResponseDTO>>> GetListByCurriculumId(long curriculumId)
    {
        return await _httpService.Get<ResponseWrapper<List<PerfectionResponseDTO>>>($"Perfection/GetListByCurriculumId/{curriculumId}");
    }

    public async Task<ResponseWrapper<PerfectionResponseDTO>> PostAsync(PerfectionDTO perfection)
    {
        return await _httpService.Post<ResponseWrapper<PerfectionResponseDTO>>($"Perfection/Post", perfection);
    }

    public async Task<ResponseWrapper<PerfectionResponseDTO>> PostRotationPerfection(long curriculumId, long rotationId, PerfectionDTO perfection)
    {
        return await _httpService.Post<ResponseWrapper<PerfectionResponseDTO>>($"Perfection/PostRotationPerfection?curriculumId={curriculumId}&rotationId={rotationId}", perfection);
    }

    public async Task<PaginationModel<PerfectionResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<PerfectionResponseDTO>>("Perfection/GetPaginateList", filter);
    }

    public async Task<ResponseWrapper<PerfectionResponseDTO>> Put(long id, PerfectionDTO perfection)
    {
        return await _httpService.Put<ResponseWrapper<PerfectionResponseDTO>>($"Perfection/Put/{id}", perfection);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Perfection/Delete/{id}");
    }

    public async Task<ResponseWrapper<byte[]>> GetExcelByteArrayClinical(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<byte[]>>($"Perfection/ExcelExportClinical/{studentId}");
    }
    public async Task<ResponseWrapper<byte[]>> GetExcelByteArrayInterventional(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<byte[]>>($"Perfection/ExcelExportInterventional/{studentId}");
    }
}
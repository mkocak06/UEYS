using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace UI.Services;

public interface IScientificStudyService
{
    Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetAll();
    Task<ResponseWrapper<ScientificStudyResponseDTO>> GetById(long id);
    Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetListByStudentId(long studentId);
    Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetForEReport(long studentId);
    Task<ResponseWrapper<ScientificStudyResponseDTO>> Add(ScientificStudyDTO scientificStudy);
    Task<ResponseWrapper<ScientificStudyResponseDTO>> Update(long id, ScientificStudyDTO scientificStudy);
    Task<PaginationModel<ScientificStudyResponseDTO>> GetPaginateList(FilterDTO filter);
    Task Delete(long id);
    Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
    Task DeleteFile(DocumentTypes documentType, long entityId);
}

public class ScientificStudyService : IScientificStudyService
{
    private readonly IHttpService _httpService;

    public ScientificStudyService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<ScientificStudyResponseDTO>>>("ScientificStudy/GetList");
    }

    public async Task<ResponseWrapper<ScientificStudyResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<ScientificStudyResponseDTO>>($"ScientificStudy/Get/{id}");
    }

    public async Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetListByStudentId(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<List<ScientificStudyResponseDTO>>>($"ScientificStudy/GetListByStudentId/{studentId}");
    }

    public async Task<ResponseWrapper<List<ScientificStudyResponseDTO>>> GetForEReport(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<List<ScientificStudyResponseDTO>>>($"EReport/GetScientificStudy/{studentId}");
    }

    public async Task<ResponseWrapper<ScientificStudyResponseDTO>> Add(ScientificStudyDTO scientificStudy)
    {
        return await _httpService.Post<ResponseWrapper<ScientificStudyResponseDTO>>($"ScientificStudy/Post", scientificStudy);
    }

    public async Task<ResponseWrapper<ScientificStudyResponseDTO>> Update(long id, ScientificStudyDTO scientificStudy)
    {
        return await _httpService.Put<ResponseWrapper<ScientificStudyResponseDTO>>($"ScientificStudy/Put/{id}", scientificStudy);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"ScientificStudy/Delete/{id}");
    }

    public async Task<PaginationModel<ScientificStudyResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ScientificStudyResponseDTO>>($"ScientificStudy/GetPaginateList", filter);
    }
    public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
    {
        return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"ScientificStudy/DownloadFile/{bucketKey}");
    }
    public async Task DeleteFile(DocumentTypes documentType, long entityId)
    {
        await _httpService.Delete($"ScientificStudy/DeleteFileS3?documentType={documentType}&entityId={entityId}");
    }
}
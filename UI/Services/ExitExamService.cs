using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.FilterModels.Base;
using Shared.Types;
using Shared.Models;

namespace UI.Services;

public interface IExitExamService
{
    Task<ResponseWrapper<List<ExitExamResponseDTO>>> GetAll();
    Task<ResponseWrapper<ExitExamRulesModel>> GetExitExamRules(long studentId);
    Task<ResponseWrapper<ExitExamResponseDTO>> GetById(long id);
    Task<PaginationModel<ExitExamResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<ExitExamResponseDTO>> GetForEReport(FilterDTO filter);
    Task<ResponseWrapper<ExitExamResponseDTO>> Add(ExitExamDTO ExitExam);
    Task<ResponseWrapper<ExitExamResponseDTO>> Update(long id, ExitExamDTO ExitExam);
    Task Delete(long id);
    Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
}

public class ExitExamService : IExitExamService
{
    private readonly IHttpService _httpService;

    public ExitExamService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<ExitExamResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<ExitExamResponseDTO>>>("ExitExam/GetList");
    }
    public async Task<PaginationModel<ExitExamResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ExitExamResponseDTO>>($"ExitExam/GetPaginateList", filter);
    }

    public async Task<PaginationModel<ExitExamResponseDTO>> GetForEReport(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ExitExamResponseDTO>>($"EReport/GetExitExam", filter);
    }

    public async Task<ResponseWrapper<ExitExamResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<ExitExamResponseDTO>>($"ExitExam/Get/{id}");
    }

    public async Task<ResponseWrapper<ExitExamResponseDTO>> Add(ExitExamDTO ExitExam)
    {
        return await _httpService.Post<ResponseWrapper<ExitExamResponseDTO>>($"ExitExam/Post", ExitExam);
    }

    public async Task<ResponseWrapper<ExitExamResponseDTO>> Update(long id, ExitExamDTO ExitExam)
    {
        return await _httpService.Put<ResponseWrapper<ExitExamResponseDTO>>($"ExitExam/Put/{id}", ExitExam);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"ExitExam/Delete/{id}");
    }
    public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
    {
        return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"ExitExam/DownloadFile/{bucketKey}");
    }

    public async Task<ResponseWrapper<ExitExamRulesModel>> GetExitExamRules(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<ExitExamRulesModel>>($"ExitExam/GetExitExamRules/{studentId}");
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.University;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface ICurriculumService
{
    Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetAll();
    Task<PaginationModel<CurriculumResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<CurriculumResponseDTO>> GetArchiveList(FilterDTO filter);
    Task<ResponseWrapper<CurriculumResponseDTO>> GetById(long id);
    Task<ResponseWrapper<CurriculumResponseDTO>> Add(CurriculumDTO curriculum);
    Task<ResponseWrapper<CurriculumResponseDTO>> CreateCopy(long id, CurriculumDTO curriculum);
    Task<ResponseWrapper<CurriculumResponseDTO>> Update(long id, CurriculumDTO curriculum);
    Task<ResponseWrapper<CurriculumResponseDTO>> Undelete(long id);
    Task Delete(long id);
    Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetByExpertiseBranchId(long expertiseBranchId);
    Task<ResponseWrapper<CurriculumResponseDTO>> GetLatestByBeginningDateAndExpertiseBranchId(long expertiseBranchId, DateTime beginningDate);
}

public class CurriculumService : ICurriculumService
{
    private readonly IHttpService _httpService;

    public CurriculumService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<CurriculumResponseDTO>>>("Curriculum/GetList");
    }

    public async Task<PaginationModel<CurriculumResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<CurriculumResponseDTO>>("Curriculum/GetPaginateList", filter);
    }

    public async Task<PaginationModel<CurriculumResponseDTO>> GetArchiveList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<CurriculumResponseDTO>>("Archive/GetCurriculumList", filter);
    }

    public async Task<ResponseWrapper<CurriculumResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<CurriculumResponseDTO>>($"Curriculum/Get/{id}");
    }

    public async Task<ResponseWrapper<CurriculumResponseDTO>> Add(CurriculumDTO curriculum)
    {
        return await _httpService.Post<ResponseWrapper<CurriculumResponseDTO>>($"Curriculum/Post", curriculum);
    }

    public async Task<ResponseWrapper<CurriculumResponseDTO>> CreateCopy(long id, CurriculumDTO curriculum)
    {
        return await _httpService.Post<ResponseWrapper<CurriculumResponseDTO>>($"Curriculum/CreateCopy/{id}", curriculum);
    }

    public async Task<ResponseWrapper<CurriculumResponseDTO>> Update(long id, CurriculumDTO curriculum)
    {
        return await _httpService.Put<ResponseWrapper<CurriculumResponseDTO>>($"Curriculum/Put/{id}", curriculum);
    }

    public async Task<ResponseWrapper<CurriculumResponseDTO>> Undelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<CurriculumResponseDTO>>($"Archive/UnDeleteCurriculum/{id}", null);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Curriculum/Delete/{id}");
    }

    public async Task<ResponseWrapper<List<CurriculumResponseDTO>>> GetByExpertiseBranchId(long expertiseBranchId)
    {
        return await _httpService.Get<ResponseWrapper<List<CurriculumResponseDTO>>>($"Curriculum/GetByExpertiseBranchId/{expertiseBranchId}");
    }

    public async Task<ResponseWrapper<CurriculumResponseDTO>> GetLatestByBeginningDateAndExpertiseBranchId(long expertiseBranchId, DateTime beginningDate)
    {
        return await _httpService.Get<ResponseWrapper<CurriculumResponseDTO>>($"Curriculum/GetLatestByBeginningDateAndExpertiseBranchId?expertiseBranchId={expertiseBranchId}&beginningDate={beginningDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")}");
    }
}
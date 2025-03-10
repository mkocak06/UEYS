using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IEducatorCountContributionFormulaService
{
    Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> GetById(long id);
    Task<PaginationModel<EducatorCountContributionFormulaResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Add(EducatorCountContributionFormulaDTO educatorCountContributionFormula);
    Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Update(long id, EducatorCountContributionFormulaDTO educatorCountContributionFormula);
    Task Delete(long id);
}

public class EducatorCountContributionFormulaService : IEducatorCountContributionFormulaService
{
    private readonly IHttpService _httpService;

    public EducatorCountContributionFormulaService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<PaginationModel<EducatorCountContributionFormulaResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<EducatorCountContributionFormulaResponseDTO>>($"EducatorCountContributionFormula/GetPaginateList", filter);
    }
    
    public async Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>>($"EducatorCountContributionFormula/Get/{id}");
    }

    public async Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Add(EducatorCountContributionFormulaDTO educatorCountContributionFormula)
    {
        return await _httpService.Post<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>>($"EducatorCountContributionFormula/Post", educatorCountContributionFormula);
    }

    public async Task<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>> Update(long id, EducatorCountContributionFormulaDTO educatorCountContributionFormula)
    {
        return await _httpService.Put<ResponseWrapper<EducatorCountContributionFormulaResponseDTO>>($"EducatorCountContributionFormula/Put/{id}", educatorCountContributionFormula);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"EducatorCountContributionFormula/Delete/{id}");
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.University;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IUniversityService
{
    Task<ResponseWrapper<List<UniversityResponseDTO>>> GetAll();
    Task<PaginationModel<UniversityResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<UniversityResponseDTO>> GetAffiliationPaginateList(FilterDTO filter);
    Task<PaginationModel<UniversityResponseDTO>> GetArchiveList(FilterDTO filter);
    Task<ResponseWrapper<UniversityResponseDTO>> GetById(long id);
    Task<ResponseWrapper<UniversityResponseDTO>> Add(UniversityDTO university);
    Task<ResponseWrapper<UniversityResponseDTO>> Update(long id, UniversityDTO university);
    Task<ResponseWrapper<UniversityResponseDTO>> Undelete(long id);
    Task Delete(long id);
    Task<ResponseWrapper<List<UniversityBreadcrumbDTO>>> GetListByExpertiseBranchId(long expBrId);
    Task<ResponseWrapper<List<UniversityBreadcrumbDTO>>> GetListByProvinceId(long expBrId);
    Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter);
    Task<ResponseWrapper<List<ReportResponseDTO>>> GetFilteredReport(List<FilterDTO> filters);
    Task<ResponseWrapper<List<ReportResponseDTO>>> GetUniversityCountByParentInstitution();
}

public class UniversityService : IUniversityService
{
    private readonly IHttpService _httpService;

    public UniversityService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<ResponseWrapper<List<UniversityResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<UniversityResponseDTO>>>("University/GetList");
    }

    public async Task<PaginationModel<UniversityResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<UniversityResponseDTO>>("University/GetPaginateList", filter);
    }
    public async Task<PaginationModel<UniversityResponseDTO>> GetAffiliationPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<UniversityResponseDTO>>("University/GetAffiliationPaginateList", filter);
    }
    public async Task<ResponseWrapper<UniversityResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<UniversityResponseDTO>>($"University/Get/{id}");
    }

    public async Task<ResponseWrapper<UniversityResponseDTO>> Add(UniversityDTO university)
    {
        return await _httpService.Post<ResponseWrapper<UniversityResponseDTO>>($"University/Post", university);
    }

    public async Task<ResponseWrapper<UniversityResponseDTO>> Update(long id, UniversityDTO university)
    {
        return await _httpService.Put<ResponseWrapper<UniversityResponseDTO>>($"University/Put/{id}", university);
    }

    public async Task<ResponseWrapper<UniversityResponseDTO>> Undelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<UniversityResponseDTO>>($"Archive/UnDeleteUniversity/{id}", null);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"University/Delete/{id}");
    }

    public async Task<ResponseWrapper<List<UniversityBreadcrumbDTO>>> GetListByProvinceId(long expBrId)
    {
        return await _httpService.Get<ResponseWrapper<List<UniversityBreadcrumbDTO>>>($"University/GetListByProvinceId/{expBrId}");
    }
    public async Task<ResponseWrapper<List<UniversityBreadcrumbDTO>>> GetListByExpertiseBranchId(long expertiseBranchId)
    {
        return await _httpService.Get<ResponseWrapper<List<UniversityBreadcrumbDTO>>>($"University/GetListByExpertiseBranchId/{expertiseBranchId}");
    }
	public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter)
	{
		return await _httpService.Post<ResponseWrapper<byte[]>>("University/ExcelExport", filter);
	}

    public async Task<PaginationModel<UniversityResponseDTO>> GetArchiveList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<UniversityResponseDTO>>("Archive/GetUniversityList", filter);

    }
    public async Task<ResponseWrapper<List<ReportResponseDTO>>> GetFilteredReport(List<FilterDTO> filters)
    {
        return await _httpService.Post<ResponseWrapper<List<ReportResponseDTO>>>($"University/GetFilteredReport", filters);
    }
    public async Task<ResponseWrapper<List<ReportResponseDTO>>> GetUniversityCountByParentInstitution()
    {
        return await _httpService.Get<ResponseWrapper<List<ReportResponseDTO>>>($"University/UniversityCountByParentInstitution");
    }
}
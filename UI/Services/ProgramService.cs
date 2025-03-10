using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IProgramService
{
    Task<ResponseWrapper<List<ProgramResponseDTO>>> GetAll();
    Task<PaginationModel<ProgramResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<ProgramResponseDTO>> GetPaginateListForProtocolProgram(FilterDTO filter);
    Task<PaginationModel<ProgramResponseDTO>> GetArchiveList(FilterDTO filter);
    Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListOnly(FilterDTO filter);
    Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListAll(FilterDTO filter);
    Task<PaginationModel<ProgramPaginateForQuotaResponseDTO>> GetPaginateListForQuotaRequest(FilterDTO filter);
    Task<PaginationModel<ProgramResponseDTO>> GetListForSearch(FilterDTO filter, bool getAll = false);
    Task<ResponseWrapper<ProgramResponseDTO>> GetById(long id);
    Task<ResponseWrapper<ProgramBreadcrumbResponseDTO>> GetWithBreadCrumb(long id);
    Task<ResponseWrapper<ProgramResponseDTO>> Add(ProgramDTO program);
    Task<ResponseWrapper<ProgramResponseDTO>> Update(long id, ProgramDTO program);
    Task<ResponseWrapper<ProgramResponseDTO>> Undelete(long id);
    Task Delete(long id);
    Task<ResponseWrapper<List<ProgramResponseDTO>>> GetProgramListByHospitalId(long hospitalId);
    Task<ResponseWrapper<List<ProgramBreadcrumbSimpleDTO>>> GetProgramListByHospitalIdBreadCrumb(long hospitalId);
    Task<ResponseWrapper<ProgramResponseDTO>> GetProgramByStudentId(long studentId);
    Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByUniversityId(long uniId);
    Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchId(long expertiseBranchId);
    Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchIdExceptOne(long expertiseBranchId);
    Task<ResponseWrapper<ProgramResponseDTO>> GetByHospitalAndExpertiseBranchId(long hospitalId, long expertiseBranchId);
    Task<ResponseWrapper<List<ProgramsLocationResponseDTO>>> GetLocationsByExpertiseBranchId(long? expBrId, long? authCategoryId);
    Task<PaginationModel<ProgramResponseDTO>> GetListForSearchByExpertiseBranchId(FilterDTO filter, long exBranchId, bool getAll = false);
    Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter);
    Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramsCountForDashboard();
    Task<ResponseWrapper<List<ProgramsCountByUniversityTypeModel>>> GetProgramsCountByUniversityType(FilterDTO filter);
    Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetFieldNamesCountForDashboard(FilterDTO filter);
    Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramCountByParentInstitution(FilterDTO filter);
    Task<ResponseWrapper<List<AuthorizationCategoryChartModel>>> CountByAuthorizationCategory(FilterDTO filter);
    Task<ResponseWrapper<List<ProgramMapModel>>> GetProgramCountByProvince(FilterDTO filter);
    Task<ResponseWrapper<bool>> CreateProgramsByHospitalId(long id);
    Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(FilterDTO filter);
}

public class ProgramService : IProgramService
{
    private readonly IHttpService _httpService;

    public ProgramService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<CountsByProfessionInstitutionModel>>>("Program/CountsByProfessionInstitution", filter);
    }
    public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<ProgramResponseDTO>>>("Program/GetList");
    }

    public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramsCountForDashboard()
    {
        return await _httpService.Get<ResponseWrapper<List<ActivePassiveResponseModel>>>("Program/GetProgramsCountForDashboard");
    }

    public async Task<ResponseWrapper<List<ProgramsCountByUniversityTypeModel>>> GetProgramsCountByUniversityType(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<ProgramsCountByUniversityTypeModel>>>("Program/GetProgramsCountByUniversityType", filter);
    }

    public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetFieldNamesCountForDashboard(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<ActivePassiveResponseModel>>>("Program/GetFieldNamesCountForDashboard", filter);
    }

    public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramCountByParentInstitution(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<ActivePassiveResponseModel>>>("Program/GetProgramCountByParentInstitution", filter);
    }

    public async Task<ResponseWrapper<List<AuthorizationCategoryChartModel>>> CountByAuthorizationCategory(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<AuthorizationCategoryChartModel>>>("Program/CountByAuthorizationCategory", filter);
    }

    public async Task<PaginationModel<ProgramResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProgramResponseDTO>>($"Program/GetPaginateList", filter);
    }

    public async Task<PaginationModel<ProgramResponseDTO>> GetPaginateListForProtocolProgram(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProgramResponseDTO>>($"Program/GetPaginateListForProtocolProgram", filter);
    }

    public async Task<PaginationModel<ProgramResponseDTO>> GetArchiveList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProgramResponseDTO>>($"Archive/GetProgramList", filter);
    }

    public async Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListOnly(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProgramPaginateResponseDTO>>($"Program/GetPaginateListOnly", filter);
    }

    public async Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListAll(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProgramPaginateResponseDTO>>($"Program/GetPaginateListAll", filter);
    }

    public async Task<PaginationModel<ProgramPaginateForQuotaResponseDTO>> GetPaginateListForQuotaRequest(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ProgramPaginateForQuotaResponseDTO>>($"Program/GetPaginateListForQuota", filter);
    }

    public async Task<PaginationModel<ProgramResponseDTO>> GetListForSearch(FilterDTO filter, bool getAll = false)
    {
        return await _httpService.Post<PaginationModel<ProgramResponseDTO>>($"Program/GetListForSearch?getAll={getAll}", filter);
    }

    public async Task<PaginationModel<ProgramResponseDTO>> GetListForSearchByExpertiseBranchId(FilterDTO filter, long exBranchId, bool getAll = false)
    {
        return await _httpService.Post<PaginationModel<ProgramResponseDTO>>($"Program/GetListForSearchByExpertiseBranchId/{exBranchId}/{getAll}", filter);
    }

    public async Task<ResponseWrapper<ProgramResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<ProgramResponseDTO>>($"Program/Get/{id}");
    }

    public async Task<ResponseWrapper<ProgramResponseDTO>> GetByHospitalAndExpertiseBranchId(long hospitalId, long expertiseBranchId)
    {
        return await _httpService.Get<ResponseWrapper<ProgramResponseDTO>>($"Program/GetByHospitalAndBranchId?hospitalId={hospitalId}&expertiseBranchId={expertiseBranchId}");
    }

    public async Task<ResponseWrapper<ProgramResponseDTO>> GetProgramByStudentId(long studentId)
    {
        return await _httpService.Get<ResponseWrapper<ProgramResponseDTO>>($"Program/GetByStudentId/{studentId}");
    }

    public async Task<ResponseWrapper<ProgramResponseDTO>> Add(ProgramDTO program)
    {
        return await _httpService.Post<ResponseWrapper<ProgramResponseDTO>>($"Program/Post", program);
    }

    public async Task<ResponseWrapper<ProgramResponseDTO>> Update(long id, ProgramDTO program)
    {
        return await _httpService.Put<ResponseWrapper<ProgramResponseDTO>>($"Program/Put/{id}", program);
    }

    public async Task<ResponseWrapper<ProgramResponseDTO>> Undelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<ProgramResponseDTO>>($"Archive/UnDeleteProgram/{id}", null);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Program/Delete/{id}");
    }

    public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetProgramListByHospitalId(long hospitalId)
    {
        return await _httpService.Get<ResponseWrapper<List<ProgramResponseDTO>>>($"Program/GetProgramListByHospitalId/{hospitalId}");
    }

    public async Task<ResponseWrapper<List<ProgramBreadcrumbSimpleDTO>>> GetProgramListByHospitalIdBreadCrumb(long hospitalId)
    {
        return await _httpService.Get<ResponseWrapper<List<ProgramBreadcrumbSimpleDTO>>>($"Program/GetProgramListByHospitalIdBreadCrumb/{hospitalId}");
    }


    public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByUniversityId(long uniId)
    {
        return await _httpService.Get<ResponseWrapper<List<ProgramResponseDTO>>>($"Program/GetListByUniversityId/{uniId}");
    }

    public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchId(long expertiseBranchId)
    {
        return await _httpService.Get<ResponseWrapper<List<ProgramResponseDTO>>>($"Program/GetListByExpertiseBranchId/{expertiseBranchId}");
    }

    public async Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchIdExceptOne(long expertiseBranchId)
    {
        return await _httpService.Get<ResponseWrapper<List<ProgramResponseDTO>>>($"Program/GetListByExpertiseBranchIdExceptOne/{expertiseBranchId}");
    }

    public async Task<ResponseWrapper<ProgramBreadcrumbResponseDTO>> GetWithBreadCrumb(long id)
    {
        return await _httpService.Get<ResponseWrapper<ProgramBreadcrumbResponseDTO>>($"Program/GetWithBreadCrumb/{id}");
    }

    public async Task<ResponseWrapper<List<ProgramsLocationResponseDTO>>> GetLocationsByExpertiseBranchId(long? expBrId, long? authCategoryId)
    {
        return await _httpService.Get<ResponseWrapper<List<ProgramsLocationResponseDTO>>>($"Program/GetLocationsByExpertiseBranchId?expBrId={expBrId}&authCategoryId={authCategoryId}");
    }

    public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<byte[]>>("Program/ExcelExport", filter);
    }

    public async Task<ResponseWrapper<List<ProgramMapModel>>> GetProgramCountByProvince(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<ProgramMapModel>>>("Program/GetProgramCountByProvince", filter);
    }

    public async Task<ResponseWrapper<bool>> CreateProgramsByHospitalId(long id)
    {
        return await _httpService.Post<ResponseWrapper<bool>>($"Program/CreateAllBranchProgramsByHospitalId/{id}", null);
    }
}
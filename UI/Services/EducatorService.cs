using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.Ekip;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Helper;

namespace UI.Services;

public interface IEducatorService
{
    Task<ResponseWrapper<List<EducatorResponseDTO>>> GetAll();
    Task<ResponseWrapper<EducatorResponseDTO>> GetById(long id);
    Task<ResponseWrapper<EducatorResponseDTO>> GetByIdForJuryList(long id);
    Task<ResponseWrapper<EducatorResponseDTO>> Add(EducatorDTO Educator);
    Task<ResponseWrapper<EducatorResponseDTO>> Update(long id, EducatorDTO Educator);
    Task<ResponseWrapper<EducatorResponseDTO>> UnDelete(long id);
    Task Delete(long id);
    Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListByHospitalId(long hospitalId);
    Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListByUniversityId(long uniId);
    Task<PaginationModel<EducatorResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<UniversityResponseDTO>> GetUniversityPaginateList(FilterDTO filter);
    Task<PaginationModel<AdvisorPaginateResponseDTO>> GetPaginateListForAdvisor(FilterDTO filter);
    Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByCore(FilterDTO filter, long studentId);
    Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListOnly(FilterDTO filter);
    Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter);
    Task<PaginationModel<EducatorPaginateResponseDTO>> GetArchiveList(FilterDTO filter);
    Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListByProgramId(FilterDTO filter, long programId);
    Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByUniversityId(FilterDTO filter, long universityId);
    Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByHospitalId(FilterDTO filter, long hospitalId);
    Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
    Task DeleteFile(DocumentTypes documentType, long entityId);
    Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByAcademicTitle(FilterDTO filter);
    Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByProfession(FilterDTO filter);
    Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByUniversityType(FilterDTO filter);
    Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(FilterDTO filter);
    Task<ResponseWrapper<List<PersonelHareketiResponseDTO>>> WorkingLifeById(long? id);
}

public class EducatorService : IEducatorService
{
    private readonly IHttpService _httpService;

    public EducatorService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<ResponseWrapper<List<EducatorResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<EducatorResponseDTO>>>("Educator/GetList");
    }

    public async Task<ResponseWrapper<EducatorResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<EducatorResponseDTO>>($"Educator/Get/{id}");
    }
    public async Task<ResponseWrapper<EducatorResponseDTO>> GetByIdForJuryList(long id)
    {
        return await _httpService.Get<ResponseWrapper<EducatorResponseDTO>>($"Educator/GetByIdForJuryList/{id}");
    }
    public async Task<ResponseWrapper<EducatorResponseDTO>> Add(EducatorDTO Educator)
    {
        return await _httpService.Post<ResponseWrapper<EducatorResponseDTO>>($"Educator/Post", Educator);
    }

    public async Task<ResponseWrapper<EducatorResponseDTO>> Update(long id, EducatorDTO Educator)
    {
        return await _httpService.Put<ResponseWrapper<EducatorResponseDTO>>($"Educator/Put/{id}", Educator);
    }

    public async Task<ResponseWrapper<EducatorResponseDTO>> UnDelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<EducatorResponseDTO>>($"Archive/UnDeleteEducator/{id}", null);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Educator/Delete/{id}");
    }

    public async Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListByHospitalId(long hospitalId)
    {
        return await _httpService.Get<ResponseWrapper<List<EducatorResponseDTO>>>($"Educator/GetListByHospitalId/{hospitalId}");
    }

    public async Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListByUniversityId(long uniId)
    {
        return await _httpService.Get<ResponseWrapper<List<EducatorResponseDTO>>>($"Educator/GetListByUniversityId/{uniId}");
    }

    public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<EducatorResponseDTO>>($"Educator/GetPaginateList", filter);
    }

    public async Task<PaginationModel<UniversityResponseDTO>> GetUniversityPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<UniversityResponseDTO>>($"Educator/GetUniversityPaginateList", filter);
    }

    public async Task<PaginationModel<AdvisorPaginateResponseDTO>> GetPaginateListForAdvisor(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<AdvisorPaginateResponseDTO>>($"Educator/GetPaginateListForAdvisor", filter);
    }

    public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByCore(FilterDTO filter, long studentId)
    {
        return await _httpService.Post<PaginationModel<EducatorResponseDTO>>($"Educator/GetPaginateListByCore?studentId={studentId}", filter);
    }

    public async Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListOnly(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<EducatorPaginateResponseDTO>>($"Educator/GetPaginateListOnly", filter);
    }

    public async Task<PaginationModel<EducatorPaginateResponseDTO>> GetArchiveList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<EducatorPaginateResponseDTO>>($"Archive/GetEducatorList", filter);
    }

    public async Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListByProgramId(FilterDTO filter, long programId)
    {
        return await _httpService.Post<PaginationModel<EducatorPaginateResponseDTO>>($"Educator/GetPaginateListByProgramId?programId={programId}", filter);
    }

    public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByUniversityId(FilterDTO filter, long unidId)
    {
        return await _httpService.Post<PaginationModel<EducatorResponseDTO>>($"Educator/GetPaginateListByUniversityId?uniId={unidId}", filter);
    }

    public async Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByHospitalId(FilterDTO filter, long hospitalId)
    {
        return await _httpService.Post<PaginationModel<EducatorResponseDTO>>($"Educator/GetPaginateListByHospitalId?hospitalId={hospitalId}", filter);
    }

    public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
    {
        return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"Educator/DownloadFile/{bucketKey}");
    }
    public async Task DeleteFile(DocumentTypes documentType, long entityId)
    {
        await _httpService.Delete($"Educator/DeleteFileS3?documentType={documentType}&entityId={entityId}");
    }

    public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<byte[]>>("Educator/ExcelExport", filter);
    }
    public async Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByAcademicTitle(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<EducatorCountByProperty>>>("Educator/CountByAcademicTitle", filter);
    }
    public async Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByProfession(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<EducatorCountByProperty>>>("Educator/CountByProfession", filter);
    }
    public async Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByUniversityType(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<EducatorCountByProperty>>>("Educator/CountByUniversityType", filter);
    }
    public async Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<CountsByProfessionInstitutionModel>>>("Educator/CountsByProfessionInstitution", filter);
    }
    public async Task<ResponseWrapper<List<PersonelHareketiResponseDTO>>> WorkingLifeById(long? id)
    {
        return await _httpService.Get<ResponseWrapper<List<PersonelHareketiResponseDTO>>>($"Educator/WorkingLifeById/{id}");
    }
}
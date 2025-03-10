using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Helper;

namespace UI.Services;

public interface IStudentService
{
    Task<ResponseWrapper<List<StudentResponseDTO>>> GetAll();
    Task<ResponseWrapper<List<BreadCrumbSearchResponseDTO>>> GetListForBreadCrumb();
    Task<ResponseWrapper<StudentResponseDTO>> GetById(long id, bool isDeleted = false);
    Task<ResponseWrapper<StudentResponseDTO>> GetRegistrationStudentById(long id);
    Task<ResponseWrapper<StudentResponseDTO>> Add(StudentDTO Student);
    Task<ResponseWrapper<StudentResponseDTO>> Update(long id, StudentDTO Student);
    Task<ResponseWrapper<StudentResponseDTO>> UnDelete(long id);
    Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter);
    Task<bool> Delete(long id, string reason = null);
    Task<ResponseWrapper<StudentResponseDTO>> CompletelyDelete(long id);
    //Task<PaginationModel<StudentResponseDTO>> GetPaginateListByProgramId(FilterDTO filter, long programId);
    //Task<PaginationModel<StudentResponseDTO>> GetPaginateListByUniversityId( FilterDTO filter, long uniId);
    //Task<PaginationModel<StudentResponseDTO>> GetPaginateListByHospitalId( FilterDTO filter, long hospitalId);
    Task<PaginationModel<StudentResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<PaginationModel<StudentResponseDTO>> GetArchiveList(FilterDTO filter);
    Task<PaginationModel<StudentPaginateResponseDTO>> GetPaginateListOnly(FilterDTO filter);
    Task<PaginationModel<StudentPaginateResponseDTO>> GetExpiredStudents();
    Task<ResponseWrapper<List<CountsByMonthsResponse>>> GetCountsByMonthsResponse();
    Task<ResponseWrapper<List<StudentCountByProperty>>> CountByUniversityType(FilterDTO filter);
    Task<ResponseWrapper<List<StudentCountByProperty>>> CountByExamType(FilterDTO filter);
    Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
    Task DeleteFile(DocumentTypes documentType, long entityId);
    Task<ResponseWrapper<List<StudentQuotaChartModel>>> CountByQuotas(FilterDTO filter);
    Task<ResponseWrapper<List<StudentCountByProperty>>> CountByProfession(FilterDTO filter);
    Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(FilterDTO filter);
}

public class StudentService : IStudentService
{
    private readonly IHttpService _httpService;

    public StudentService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<CountsByProfessionInstitutionModel>>>("Student/CountsByProfessionInstitution", filter);
    }
    public async Task<ResponseWrapper<List<StudentCountByProperty>>> CountByProfession(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<StudentCountByProperty>>>("Student/CountByProfession", filter);
    }
    public async Task<ResponseWrapper<List<CountsByMonthsResponse>>> GetCountsByMonthsResponse()
    {
        return await _httpService.Get<ResponseWrapper<List<CountsByMonthsResponse>>>("Student/GetCountsByMonthsResponse");
    }
    public async Task<ResponseWrapper<List<StudentCountByProperty>>> CountByUniversityType(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<StudentCountByProperty>>>("Student/CountByUniversityType", filter);
    }
    public async Task<ResponseWrapper<List<StudentQuotaChartModel>>> CountByQuotas(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<StudentQuotaChartModel>>>("Student/CountByQuotas", filter);
    }
    public async Task<ResponseWrapper<List<StudentCountByProperty>>> CountByExamType(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<StudentCountByProperty>>>("Student/CountByExamType", filter);
    }
    public async Task<ResponseWrapper<List<StudentResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<StudentResponseDTO>>>("Student/GetList");
    }

    public async Task<ResponseWrapper<List<BreadCrumbSearchResponseDTO>>> GetListForBreadCrumb()
    {
        return await _httpService.Get<ResponseWrapper<List<BreadCrumbSearchResponseDTO>>>("Student/GetListForBreadCrumb");
    }

    public async Task<ResponseWrapper<StudentResponseDTO>> GetById(long id, bool isDeleted = false)
    {
        return await _httpService.Get<ResponseWrapper<StudentResponseDTO>>($"Student/Get?id={id}&isDeleted={isDeleted}");
    }

    public async Task<ResponseWrapper<StudentResponseDTO>> GetRegistrationStudentById(long id)
    {
        return await _httpService.Get<ResponseWrapper<StudentResponseDTO>>($"Student/GetRegistrationStudentById?id={id}");
    }

    public async Task<ResponseWrapper<StudentResponseDTO>> Add(StudentDTO Student)
    {
        return await _httpService.Post<ResponseWrapper<StudentResponseDTO>>($"Student/Post", Student);
    }

    public async Task<ResponseWrapper<StudentResponseDTO>> Update(long id, StudentDTO Student)
    {
        return await _httpService.Put<ResponseWrapper<StudentResponseDTO>>($"Student/Put/{id}", Student);
    }

    public async Task<ResponseWrapper<StudentResponseDTO>> UnDelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<StudentResponseDTO>>($"Archive/UnDeleteStudent/{id}", null);
    }

    public async Task<bool> Delete(long id, string reason = null)
    {
        return await _httpService.Delete($"Student/Delete/{id}?reason={reason}");
    }

    public async Task<ResponseWrapper<StudentResponseDTO>> CompletelyDelete(long id)
    {
        return await _httpService.Put<ResponseWrapper<StudentResponseDTO>>($"Student/CompletelyDelete/{id}", null);
    }

    //public async Task<PaginationModel<StudentResponseDTO>> GetPaginateListByProgramId(FilterDTO filter, long programId)
    //{
    //    return await _httpService.Post<PaginationModel<StudentResponseDTO>>($"Student/GetPaginateListByProgramId?programId={programId}", filter);
    //}

    //public async Task<PaginationModel<StudentResponseDTO>> GetPaginateListByUniversityId(FilterDTO filter, long unidId)
    //{
    //    return await _httpService.Post<PaginationModel<StudentResponseDTO>>($"Student/GetPaginateListByUniversityId?uniId={unidId}", filter);
    //}

    //public async Task<PaginationModel<StudentResponseDTO>> GetPaginateListByHospitalId(FilterDTO filter, long hospitalId)
    //{
    //    return await _httpService.Post<PaginationModel<StudentResponseDTO>>($"Student/GetPaginateListByHospitalId?hospitalId={hospitalId}", filter);
    //}

    public async Task<PaginationModel<StudentResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<StudentResponseDTO>>($"Student/GetPaginateList", filter);
    }

    public async Task<PaginationModel<StudentResponseDTO>> GetArchiveList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<StudentResponseDTO>>($"Archive/GetStudentList", filter);
    }

    public async Task<PaginationModel<StudentPaginateResponseDTO>> GetPaginateListOnly(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<StudentPaginateResponseDTO>>($"Student/GetPaginateListOnly", filter);
    }
    public async Task<PaginationModel<StudentPaginateResponseDTO>> GetExpiredStudents()
    {
        return await _httpService.Get<PaginationModel<StudentPaginateResponseDTO>>($"Student/GetExpiredStudents");
    }


    public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
    {
        return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"Student/DownloadFile/{bucketKey}");
    }

    public async Task DeleteFile(DocumentTypes documentType, long entityId)
    {
        await _httpService.Delete($"Student/DeleteFileS3?documentType={documentType}&entityId={entityId}");
    }

    public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter)
    {
        return await _httpService.ExcelByteArray("Student/ExcelExport", filter);
    }
}
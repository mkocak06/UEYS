using Shared.BaseModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IStudentService
    {
        Task<ResponseWrapper<List<StudentResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginationModel<StudentResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        //Task<ResponseWrapper<List<StudentResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<ResponseWrapper<StudentResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentDTO studentDTO);
        Task<ResponseWrapper<StudentResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id, bool isDeleted = false);
        Task<ResponseWrapper<StudentResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentDTO studentDTO);
        Task<ResponseWrapper<StudentResponseDTO>> Delete(CancellationToken cancellationToken, long id, string delete);
		Task<ResponseWrapper<StudentResponseDTO>> CompletelyDelete(CancellationToken cancellationToken, long id);
		Task<ResponseWrapper<StudentResponseDTO>> AddStudentToProgram(CancellationToken cancellationToken, long studentId, long programId);
        Task<ResponseWrapper<List<StudentResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId);
        Task<PaginationModel<StudentPaginateResponseDTO>> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<BreadCrumbSearchResponseDTO>>> GetListForBreadCrumb(CancellationToken cancellationToken);
        ResponseWrapper<List<CountsByMonthsResponse>> GetCountsByMonthsResponse();
        Task<ResponseWrapper<StudentResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<StudentCountByProperty>>> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<StudentCountByProperty>>> CountByExamType(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<StudentQuotaChartModel>>> CountByQuotas(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<StudentCountByProperty>>> CountByProfession(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<StudentResponseDTO>> GetRegistrationStudentById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<RestartStudentUserModel>>> GetRestartStudents(CancellationToken cancellationToken);
        Task<PaginationModel<StudentPaginateResponseDTO>> GetExpiredStudents(CancellationToken cancellationToken);
    
    }
}

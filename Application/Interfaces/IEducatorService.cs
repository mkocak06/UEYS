using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.Ekip;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IEducatorService 
    {
        Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginationModel<EducatorResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByCore(CancellationToken cancellationToken, FilterDTO filter, long studentId);
        Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter);
        Task<PaginationModel<EducatorPaginateResponseDTO>> GetPaginateListByProgramId(CancellationToken cancellationToken, FilterDTO filter, long programId);
        Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByHospitalId(CancellationToken cancellationToken, FilterDTO filter, long hospitalId);
        Task<PaginationModel<EducatorResponseDTO>> GetPaginateListByUniversityId(CancellationToken cancellationToken, FilterDTO filter, long uniId);
        Task<ResponseWrapper<EducatorResponseDTO>> PostAsync(CancellationToken cancellationToken, EducatorDTO educatorDTO);
        Task<ResponseWrapper<EducatorResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<EducatorResponseDTO>> Put(CancellationToken cancellationToken, long id, EducatorDTO educatorDTO);
        Task<ResponseWrapper<List<EducatorResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<ResponseWrapper<EducatorResponseDTO>> GetByIdentityNo(CancellationToken cancellationToken, string identityNo);
        Task<PaginationModel<AdvisorPaginateResponseDTO>> GetPaginateListForAdvisor(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<EducatorResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByAcademicTitle(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByProfession(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<EducatorCountByProperty>>> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<EducatorResponseDTO>> GetByIdForJuryList(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<PersonelHareketiResponseDTO>>> WorkingLifeById(CancellationToken cancellationToken, long educatorId);
    }
}

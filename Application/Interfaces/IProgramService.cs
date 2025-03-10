using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProgramService
    {
        //PaginationModel<ProgramResponseDTO> GetList(FilterDTO filter);
        Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchIdAsync(CancellationToken cancellationToken, long expertiseBranchId);
        Task<ResponseWrapper<ProgramResponseDTO>> GetByHospitalAndBranchIdAsync(CancellationToken cancellationToken, long hospitalId, long expertiseBranchId);
        Task<ResponseWrapper<ProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, ProgramDTO programDTO);
        Task<ResponseWrapper<ProgramResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ProgramResponseDTO>> GetByStudentIdAsync(CancellationToken cancellationToken, long studentId);
        Task<ResponseWrapper<ProgramBreadcrumbResponseDTO>> GetByIdWithBreadcrumbAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, ProgramDTO programDTO);
        Task<ResponseWrapper<ProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ProgramResponseDTO>> ImportFromExcel(CancellationToken cancellationToken, IFormFile formFile);
        Task<PaginationModel<ProgramResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListOnly(CancellationToken cancellationToken, FilterDTO filter);
        Task<PaginationModel<ProgramPaginateResponseDTO>> GetPaginateListAll(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByHospitalIdAsync(CancellationToken cancellationToken, long hospitalId);
        Task<ResponseWrapper<List<ProgramBreadcrumbSimpleDTO>>> GetProgramListByHospitalIdBreadCrumb(CancellationToken cancellationToken, long hospitalId);
        Task<ResponseWrapper<List<ProgramsLocationResponseDTO>>> GetLocationsByExpertiseBranchId(CancellationToken cancellationToken, long? expBrId, long? authCategoryId);
        Task<ResponseWrapper<List<ProgramResponseDTO>>> GetListByExpertiseBranchIdExceptOneAsync(CancellationToken cancellationToken, long expertiseBranchId);
        Task<PaginationModel<ProgramResponseDTO>> GetListForSearch(CancellationToken cancellationToken, FilterDTO filter, bool getAll);
        Task<PaginationModel<ProgramResponseDTO>> GetListForSearchByExpertiseBranchId(CancellationToken cancellationToken, FilterDTO filter, long exBranchId, bool getAll = false);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramsForDashboard(CancellationToken cancellationToken);
        Task<ResponseWrapper<ProgramResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<bool>> AddProgramsByExpertiseBranches(CancellationToken cancellationToken);
        Task<PaginationModel<ProgramResponseDTO>> GetPaginateListForProtocolProgram(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ProgramsCountByUniversityTypeModel>>> CountByUniversityType(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetFieldNamesCountForDashboard(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<AuthorizationCategoryChartModel>>> CountByAuthorizationCategory(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ProgramMapModel>>> GetProgramCountByProvince(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetProgramCountByParentInstitution(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<bool>> CreateAllBranchProgramsByHospitalId(CancellationToken cancellationToken, long hospitalId);
        Task<ResponseWrapper<List<CountsByProfessionInstitutionModel>>> CountsByProfessionInstitution(CancellationToken cancellationToken, FilterDTO filter);
        Task<PaginationModel<ProgramPaginateForQuotaResponseDTO>> GetPaginateListForQuota(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<string>>> YUEPExcelCheck(CancellationToken cancellationToken, IFormFile formFile);
    }
}

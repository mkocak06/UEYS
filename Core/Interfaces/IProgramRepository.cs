using System;
using Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Shared.ResponseModels.Program;
using Shared.FilterModels.Base;
using Shared.ResponseModels.StatisticModels;
using Core.Models.Authorization;
using Core.Models.DetailedReportModels;
using Shared.FilterModels;
using Shared.ResponseModels;

namespace Core.Interfaces
{
    public interface IProgramRepository : IRepository<Program>
    {
        IQueryable<Program> QueryablePrograms(Expression<Func<Program, bool>> predicate);
        IQueryable<ProgramPaginateResponseDTO> PaginateListQuery(ZoneModel zone, Expression<Func<Program, bool>> predicate = null, bool isSearch = false);
        Task<Program> GetWithSubRecords(CancellationToken cancellationToken, long id);
        Task<List<Program>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<List<Program>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId);
        Task<List<ProgramBreadcrumbSimpleDTO>> GetProgramListByHospitalIdBreadCrumb(CancellationToken cancellationToken, long hospitalId);
        Task<List<Program>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId);
        Task<ProgramResponseDTO> GetByHospitalAndBranchId(CancellationToken cancellationToken, long hospitalId, long expertiseBranchId);
        Task<List<ProgramExpertiseBreadcrumbResponseDTO>> GetListByUniversityIdBreadCrumbModel(CancellationToken cancellationToken, long uniId);
        Task<Program> GetByStudentId(CancellationToken cancellationToken, long studentId);
        Task<List<ProgramsLocationResponseDTO>> GetLocationsByExpertiseBranchId(CancellationToken cancellationToken, long? expBrId, long? authCategoryId);
        List<ActivePassiveResponseModel> ProgramCountForDashboard();
        IQueryable<ProgramExportResponseDTO> ExportListQuery(ZoneModel zone);
        Task<ProgramResponseDTO> CheckProtocolProgram(CancellationToken cancellationToken, long id);
        IQueryable<ProgramChartModel> QueryableProgramsForCharts(ZoneModel zone);
        Task<List<ProgramStaffModel>> ProgramStaffInfo(CancellationToken cancellationToken);
        long ProgramCountByInstitution(ZoneModel zone, long parentInstitutionId);
        IQueryable<ProgramPaginateForQuotaResponseDTO> GetProgramsForQuota(ZoneModel zone);
        Task<List<ProgramsStaffCount>> GetProgramsForExcelExport(ZoneModel zone, ProgramFilter filter);
        IQueryable<ProgramPaginateResponseDTO> AllPaginateListQuery();
        Task<List<CountByExpertiseBranch>> CountByExpertiseBranch(CancellationToken cancellationToken, ZoneModel zone,
            ProgramFilter filter);
    }
}

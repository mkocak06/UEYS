using Core.Entities;
using Core.Models.Authorization;
using Core.Models.Educator;
using Shared.FilterModels;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Educator;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEducatorRepository : IRepository<Educator>
    {
        IQueryable<EducatorPaginateResponseDTO> OnlyPaginateListQuery(ZoneModel zone);
        IQueryable<Educator> GetByUniversityIdQuery(long uniId);
        IQueryable<Educator> GetByProgramIdQuery(long programId);
        IQueryable<Educator> GetByHospitalIdQuery(long hospitalId);
        IQueryable<Educator> PaginateListQuery();
        IQueryable<Educator> PaginateListQueryByCore(long studentId);
        Task<Educator> GetWithSubRecords(CancellationToken cancellationToken, long id);
        Task<Educator> UpdateWithSubRecords(CancellationToken cancellationToken, long id, Educator educator);
        IQueryable<EducatorExp> PaginateListForAdvisorQuery(long eduplaceId);
        IQueryable<Educator> QueryList(ZoneModel zone);
        IQueryable<EducatorChartModel> QueryableEducatorsForCharts(ZoneModel zone);
        long EducatorCountByInstitution(ZoneModel zone, long parentInstitutionId);
        Task<Educator> GetWithSubRecordsWithZone(CancellationToken cancellationToken, ZoneModel zone, long id);
        Task<Educator> GetByIdForJuryList(CancellationToken cancellationToken, long id);
        Task<List<EducatorPaginateResponseDTO>> ListForExcelQuery(ZoneModel zone, ProgramFilter filter);
        IQueryable<EducatorPaginateResponseDTO> GetAllEducatorsForExport();
        Task DeleteEducator(CancellationToken cancellationToken, EducatorDTO educatorDTO);
        Task UnDeleteEducator(CancellationToken cancellationToken, long id);
        Task<EducatorModel> EducatorTest(CancellationToken cancellationToken, string tckn);
    }
}

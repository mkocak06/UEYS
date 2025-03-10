using Core.Entities;
using Core.Models.Authorization;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.University;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUniversityRepository : IRepository<University>
    {
        Task<University> GetByIdWithSubRecords(CancellationToken cancellationToken, long id);
        IQueryable<University> QueryableUniversities(Expression<Func<University, bool>> predicate);
        Task<List<UniversityBreadcrumbDTO>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId);
        long UniversityCountByInstitution(ZoneModel zone, long parentInstitutionId);
        List<ReportResponseDTO> GetReports();
        IQueryable<University> QueryableUniversitiesForAffilitation(); 
        IQueryable<University> PaginateQuery(ZoneModel zone);
    }
}
 
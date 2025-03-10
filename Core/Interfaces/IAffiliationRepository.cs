using Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.ResponseModels;
using Core.Models.Authorization;

namespace Core.Interfaces
{
    public interface IAffiliationRepository : IRepository<Affiliation>
    {
        IQueryable<Affiliation> QueryableAffiliations();
        Task<Affiliation> GetWithSubRecords(CancellationToken cancellationToken, long id);
        Task<List<Affiliation>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId);
        Task<List<Affiliation>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<Affiliation> GetWithFacultyHospitalId(CancellationToken cancellationToken, long facultyid, long hospitalid);
        Task<List<AffiliationExcelExport>> ExcelExport(CancellationToken cancellationToken);
        IQueryable<Affiliation> PaginateQuery(ZoneModel zone);
    }
}

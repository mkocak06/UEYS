using Core.Entities;
using Core.Models.Authorization;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHospitalRepository : IRepository<Hospital>
    {
        Task<List<Hospital>> GetListByUniversityId(long uniId);
        Task<List<HospitalBreadcrumbDTO>> GetListByExpertiseBranchId(CancellationToken cancellationToken, long expertiseBranchId);
        Task<List<MapDTO>> GetListForMap(CancellationToken cancellationToken, long? universityId);
        IQueryable<Hospital> QueryableHospitals();
        Task<UserHospitalDetailDTO> GetUserHospitalDetail(CancellationToken cancellationToken, long userId);
        Task<Hospital> GetWithSubRecords(CancellationToken cancellationToken, long id);
        IQueryable<HospitalChartModel> QueryableHospitalsForCharts(ZoneModel zone);
        long HospitalCountByInstitution(ZoneModel zone, long parentInstitutionId);
    }
}

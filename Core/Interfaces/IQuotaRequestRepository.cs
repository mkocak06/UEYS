using System.Linq;
using Core.Entities;
using System.Threading;
using System.Threading.Tasks;
using Core.Models.Authorization;

namespace Core.Interfaces
{
    public interface IQuotaRequestRepository : IRepository<QuotaRequest>
    {
        Task<QuotaRequest> GetWithSubRecords(CancellationToken cancellationToken, long id, ZoneModel zone);
        IQueryable<QuotaRequest> GetListWithSubRecords(CancellationToken cancellationToken);

    }
}

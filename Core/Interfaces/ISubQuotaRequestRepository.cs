using System.Linq;
using Core.Entities;
using System.Threading;
using System.Threading.Tasks;
using Shared.ResponseModels;

namespace Core.Interfaces
{
    public interface ISubQuotaRequestRepository : IRepository<SubQuotaRequest>
    {
        Task<SubQuotaRequest> GetWithSubRecords(CancellationToken cancellationToken, long id);
        Task<SubQuotaRequest> GetByProgramIdWithSubRecords(CancellationToken cancellationToken, long programId);
        IQueryable<SubQuotaRequestPaginateResponseDTO> PaginateQuery();
    }
}

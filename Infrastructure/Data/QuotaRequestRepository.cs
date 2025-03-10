using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Core.Models.Authorization;
using Core.Extentsions;
using Shared.Types;

namespace Infrastructure.Data
{
    public class QuotaRequestRepository : EfRepository<QuotaRequest>, IQuotaRequestRepository
    {
        private readonly ApplicationDbContext dbContext;
        public QuotaRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<QuotaRequest> GetWithSubRecords(CancellationToken cancellationToken, long id, ZoneModel zone)
        {
            IQueryable<QuotaRequest> quotaRequest = dbContext.QuotaRequests.AsSplitQuery()
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.StudentCounts)
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.SubQuotaRequestPortfolios).ThenInclude(x=>x.Portfolio)
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province);

            if (zone.Hospitals != null && zone.Hospitals.Any())
            {
                var hospitalIds = zone.Hospitals.Select(x => x.Id).ToList();
                quotaRequest = quotaRequest.Include(x => x.SubQuotaRequests.Where(x => x.IsDeleted == false && hospitalIds.Contains(x.Program.HospitalId.Value))).ThenInclude(x => x.StudentCounts)
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.SubQuotaRequestPortfolios).ThenInclude(x => x.Portfolio)
                .Include(x => x.SubQuotaRequests.Where(x => x.IsDeleted == false && hospitalIds.Contains(x.Program.HospitalId.Value))).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x => x.Profession)
                .Include(x => x.SubQuotaRequests.Where(x => x.IsDeleted == false && hospitalIds.Contains(x.Program.HospitalId.Value))).ThenInclude(x => x.Program).ThenInclude(x => x.Hospital).ThenInclude(x => x.Province);
            }
            return await quotaRequest.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id, cancellationToken);
        }
        public IQueryable<QuotaRequest> GetListWithSubRecords(CancellationToken cancellationToken)
        {
            return dbContext.QuotaRequests.AsSplitQuery()
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.SubQuotaRequestPortfolios).ThenInclude(x => x.Portfolio)
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.Program).ThenInclude(x => x.ExpertiseBranch)
                .Include(x => x.SubQuotaRequests).ThenInclude(x => x.StudentCounts);
        }
    }
}

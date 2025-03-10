using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class SubQuotaRequestPortfolioRepository : EfRepository<SubQuotaRequestPortfolio>, ISubQuotaRequestPortfolioRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SubQuotaRequestPortfolioRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}

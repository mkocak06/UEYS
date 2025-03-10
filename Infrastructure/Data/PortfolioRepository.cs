using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class PortfolioRepository : EfRepository<Portfolio>, IPortfolioRepository
    {
        private readonly ApplicationDbContext dbContext;
        public PortfolioRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}

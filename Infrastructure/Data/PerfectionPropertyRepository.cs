using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class PerfectionPropertyRepository : EfRepository<PerfectionProperty>, IPerfectionPropertyRepository
    {
        private readonly ApplicationDbContext dbContext;

        public PerfectionPropertyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
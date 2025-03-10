using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class DependentProgramRepository : EfRepository<DependentProgram>, IDependentProgramRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DependentProgramRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}

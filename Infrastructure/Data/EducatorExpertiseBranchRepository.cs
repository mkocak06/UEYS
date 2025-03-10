using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class EducatorExpertiseBranchRepository : EfRepository<EducatorExpertiseBranch>, IEducatorExpertiseBranchRepository
    {
        private readonly ApplicationDbContext dbContext;

        public EducatorExpertiseBranchRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}

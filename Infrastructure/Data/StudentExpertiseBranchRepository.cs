using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class StudentExpertiseBranchRepository : EfRepository<StudentExpertiseBranch>, IStudentExpertiseBranchRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StudentExpertiseBranchRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}

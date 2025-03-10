using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class StudentCountRepository : EfRepository<StudentCount>, IStudentCountRepository
    {
        private readonly ApplicationDbContext dbContext;
        public StudentCountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}

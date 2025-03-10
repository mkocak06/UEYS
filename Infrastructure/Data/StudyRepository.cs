using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class StudyRepository : EfRepository<Study>, IStudyRepository
    {
        public StudyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

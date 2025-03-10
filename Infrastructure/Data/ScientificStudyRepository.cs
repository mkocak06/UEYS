using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class ScientificStudyRepository : EfRepository<ScientificStudy>, IScientificStudyRepository
    {
        public ScientificStudyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

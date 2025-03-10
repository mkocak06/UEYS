using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data
{
    public class CurriculumPerfectionRepository : EfRepository<CurriculumPerfection>, ICurriculumPerfectionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CurriculumPerfectionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<CurriculumPerfection> GetWithSubRecords()
        {
            return dbContext.CurriculumPerfections.AsSplitQuery().AsNoTracking()
                                                  .Include(x => x.Curriculum).ThenInclude(x => x.ExpertiseBranch)
                                                  .Include(x => x.Perfection).ThenInclude(x => x.PerfectionProperties).ThenInclude(x => x.Property);
        }
    }
}
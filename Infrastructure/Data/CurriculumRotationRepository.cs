using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data
{
    public class CurriculumRotationRepository : EfRepository<CurriculumRotation>, ICurriculumRotationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CurriculumRotationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<CurriculumRotation> GetWithSubRecords()
        {
            return dbContext.CurriculumRotations.AsSplitQuery().AsNoTracking()
                                                  .Include(x => x.Curriculum).ThenInclude(x => x.ExpertiseBranch)
                                                  .Include(x => x.Rotation).ThenInclude(x => x.ExpertiseBranch)
                                                  .Include(x=>x.Perfections).ThenInclude(x=>x.PerfectionProperties).ThenInclude(x => x.Property);
        }
    }
}
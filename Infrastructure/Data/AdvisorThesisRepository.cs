using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AdvisorThesisRepository : EfRepository<AdvisorThesis>, IAdvisorThesisRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AdvisorThesisRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdvisorThesis> GetById(CancellationToken cancellationToken, long id)
        {
            return await dbContext.AdvisorTheses.AsNoTracking().AsSplitQuery()
                .Include(x => x.ExpertiseBranch)
                .Include(x => x.Educator.EducatorExpertiseBranches).ThenInclude(x => x.ExpertiseBranch)
                .Include(x=>x.Educator.User)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

    }
}

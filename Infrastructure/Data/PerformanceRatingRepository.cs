using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class PerformanceRatingRepository : EfRepository<PerformanceRating>, IPerformanceRatingRepository
    {
        private readonly ApplicationDbContext dbContext;
        public PerformanceRatingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<PerformanceRating>> GetListByStudentId(CancellationToken cancellationToken, long studentId)
        {
            return await dbContext.PerformanceRatings.AsSplitQuery()
                                                     .Include(x => x.Educator).ThenInclude(x => x.User)
                                                     .Where(x => x.StudentId == studentId && x.IsDeleted == false)
                                                     .ToListAsync(cancellationToken);
        }
        public async Task<PerformanceRating> GetByRatingId(CancellationToken cancellationToken, long id)
        {
            return await dbContext.PerformanceRatings.AsSplitQuery().AsNoTracking()
                                                     .Include(x => x.Educator).ThenInclude(x => x.User)
                                                     .Include(x => x.Student).ThenInclude(x => x.User)
                                                     .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
        }

        public async Task<EducatorProgram> GetEducatorByStudentId(CancellationToken cancellationToken, long? studentId, long userId)
        {
            var query = from s in dbContext.Students
                        join p in dbContext.Programs on s.ProgramId equals p.Id
                        join ep in dbContext.EducatorPrograms on p.Id equals ep.ProgramId
                        where s.Id == studentId && ep.Educator.UserId == userId
                        select ep;
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}

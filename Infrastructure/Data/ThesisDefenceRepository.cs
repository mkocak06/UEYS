using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;

namespace Infrastructure.Data
{
    public class ThesisDefenceRepository : EfRepository<ThesisDefence>, IThesisDefenceRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ThesisDefenceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<ThesisDefence> GetWithSubRecords(Expression<Func<ThesisDefence, bool>> predicate, CancellationToken cancellationToken)
        {
            {
                return await dbContext.ThesisDefences.AsSplitQuery().AsNoTracking()
                    .Include(x => x.Juries)
                    .FirstOrDefaultAsync(predicate, cancellationToken);
            }
        }

        public async Task<ThesisDefence> GetById(long id, CancellationToken cancellationToken)
        {
            {
                return await dbContext.ThesisDefences.AsSplitQuery().AsNoTracking()
                    .Include(x=>x.Hospital)
                    .Include(x => x.Juries.Where(x=>!x.IsDeleted)).ThenInclude(x=>x.User)
                    .Include(x => x.Juries.Where(x => !x.IsDeleted)).ThenInclude(x=>x.Educator.User)
                    .Include(x => x.Juries.Where(x => !x.IsDeleted)).ThenInclude(x=>x.Educator.EducatorExpertiseBranches).ThenInclude(x=>x.ExpertiseBranch)
                    .FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);
            }
        }
        public async Task<ThesisDefence> UpdateWithSubRecords(CancellationToken cancellationToken, long id, ThesisDefence thesisDefence)
        {
            ThesisDefence existThesisDefence = await GetWithSubRecords(x => x.Id == id, cancellationToken);
            dbContext.Entry(thesisDefence).State = EntityState.Modified;

            if (thesisDefence.Juries != null)
            {

                foreach (var item in thesisDefence.Juries)
                {
                    var existJury = existThesisDefence.Juries.FirstOrDefault(x => x.Id == item.Id);
                    dbContext.Entry(item).State = existJury == null ? EntityState.Added : EntityState.Modified;

                }
                var JuriesIds = thesisDefence.Juries.Select(r => r.Id).ToList();
                if (existThesisDefence?.Juries != null)
                {
                    foreach (var item in existThesisDefence.Juries.Where(x => !JuriesIds.Contains(x.Id)))
                    {
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return thesisDefence;
        }

    }
}

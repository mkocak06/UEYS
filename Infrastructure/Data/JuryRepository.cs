using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class JuryRepository : EfRepository<Jury>, IJuryRepository
    {
        private readonly ApplicationDbContext dbContext;

        public JuryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Jury> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Juries.AsSplitQuery()
                                           .Include(x => x.Educator)
                                           .Include(x => x.ThesisDefence)
                                           .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false && x.Educator.IsDeleted == false && x.ThesisDefence.IsDeleted == false, cancellationToken);
        }
    }
}

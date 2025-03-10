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
    public class EducatorDependentProgramRepository : EfRepository<EducatorDependentProgram>, IEducatorDependentProgramRepository
    {
        private readonly ApplicationDbContext dbContext;

        public EducatorDependentProgramRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<EducatorDependentProgram>> GetListWithSubRecords(CancellationToken cancellationToken, long dependProgId)
        {
            return await dbContext.EducatorDependentPrograms.AsQueryable()
                                           .Include(x => x.Educator)
                                           .Include(x => x.DependentProgram)
                                           .Where(x => x.DependentProgramId == dependProgId && x.Educator.IsDeleted == false)
                                           .ToListAsync(cancellationToken);
        }
    }
}

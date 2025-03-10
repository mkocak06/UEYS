using Core.Entities;
using Core.Interfaces;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class FacultyRepository : EfRepository<Faculty>, IFacultyRepository
    {
        private readonly ApplicationDbContext dbContext;

        public FacultyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Faculty>> GetListByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            return await dbContext.Faculties.AsSplitQuery()
                                            .Include(x => x.Programs).ThenInclude(x => x.ExpertiseBranch).ThenInclude(x=>x.Profession)
                                            .Where(x => x.UniversityId == uniId)
                                            .ToListAsync(cancellationToken);
        }
    }
}

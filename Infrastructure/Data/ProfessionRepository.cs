using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProfessionRepository : EfRepository<Profession>, IProfessionRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ProfessionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Profession>> GetByUniversityId(CancellationToken cancellationToken, long uniId)
        {
            return await (from p in dbContext.Professions
                          join eb in dbContext.ExpertiseBranches on p.Id equals eb.ProfessionId
                          join prog in dbContext.Programs on eb.Id equals prog.ExpertiseBranchId
                          where prog.Faculty.UniversityId == uniId
                          select p).Distinct().ToListAsync(cancellationToken);
        }

        public async Task<Profession> GetWithSubRecords(CancellationToken cancellationToken, long id)
        {
            return await dbContext.Professions.AsSplitQuery()
                                              .Include(x => x.ExpertiseBranches).ThenInclude(x => x.Programs).ThenInclude(x => x.Faculty)
                                              .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}

using Core.Entities;
using Core.Interfaces;
using System.Linq;
using System.Threading;

namespace Infrastructure.Data
{
    public class EducatorProgramRepository : EfRepository<EducatorProgram>, IEducatorProgramRepository
    {
        private readonly ApplicationDbContext dbContext;

        public EducatorProgramRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<EducatorProgram> GetListByHospitalId(long hospitalId)
        {
            return (from ep in dbContext.EducatorPrograms
                    join p in dbContext.Programs on ep.ProgramId equals p.Id
                    join eeb in dbContext.EducatorExpertiseBranches on ep.EducatorId equals eeb.EducatorId
                    where eeb.ExpertiseBranchId == 49 && p.HospitalId == hospitalId
                    select ep);
        }
    }
}

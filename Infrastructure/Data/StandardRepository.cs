using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.ResponseModels.Standard;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StandardRepository : EfRepository<Standard>, IStandardRepository
    {
        private readonly ApplicationDbContext dbContext;
        public StandardRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<ProgramStandardResponseDTO>> GetByLatestCurriculumsExpertiseBranch(long expBranchId)
        {
            var latestCurriculum = dbContext.Curricula.Where(c => c.ExpertiseBranchId == expBranchId).MaxBy(c => c.EffectiveDate);

            return await dbContext.Standards.AsSplitQuery().AsNoTracking()
                                            .Where(x => x.CurriculumId == latestCurriculum.Id)
                                            .Select(x => new ProgramStandardResponseDTO()
                                            {
                                                Id = x.Id,
                                                Name = x.Name,
                                                ExpertiseBranchName = x.Curriculum.ExpertiseBranch.Name,
                                                ExpertiseBranchId = x.Curriculum.ExpertiseBranch.Id,
                                                ExpertiseBranchVersion = x.Curriculum.Version,
                                                StandardCategory = x.StandardCategory.Name,
                                                Description = x.Description
                                            })
                                            .ToListAsync();
        }
    }
}

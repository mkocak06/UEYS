using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class EducatorCountContributionFormulaRepository : EfRepository<EducatorCountContributionFormula>, IEducatorCountContributionFormulaRepository
    {
        private readonly ApplicationDbContext dbContext;
        public EducatorCountContributionFormulaRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
       
    }
}

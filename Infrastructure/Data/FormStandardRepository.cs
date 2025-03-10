using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class FormStandardRepository : EfRepository<FormStandard>, IFormStandardRepository
    {
        public FormStandardRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

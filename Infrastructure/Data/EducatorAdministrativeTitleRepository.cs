using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class EducatorAdministrativeTitleRepository : EfRepository<EducatorAdministrativeTitle>, IEducatorAdministrativeTitleRepository
    {
        private readonly ApplicationDbContext dbContext;

        public EducatorAdministrativeTitleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}

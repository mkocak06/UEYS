using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class EducatorStaffInstitutionRepository : EfRepository<EducatorStaffInstitution>, IEducatorStaffInstitutionRepository
    {
        public EducatorStaffInstitutionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

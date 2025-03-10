using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class AuthorizationDetailRepository : EfRepository<AuthorizationDetail>, IAuthorizationDetailRepository
    {
        public AuthorizationDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
 
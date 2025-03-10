using Core.Entities.Koru;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class RoleCategoryRepository : EfRepository<RoleCategory>, IRoleCategoryRepository
    {
        public RoleCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

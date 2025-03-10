using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StandardCategoryRepository : EfRepository<StandardCategory> , IStandardCategoryRepository
    {
        public StandardCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

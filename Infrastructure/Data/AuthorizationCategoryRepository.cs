using Core.Entities;
using Core.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;

namespace Infrastructure.Data
{
    public class AuthorizationCategoryRepository : EfRepository<AuthorizationCategory>, IAuthorizationCategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthorizationCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        //public int GetOrderNumber()
        //{
        //    return _dbContext.AuthorizationCategories.Max(x => x.Order) + 1;
        //}

        //public async Task ChangeOrder(CancellationToken cancellationToken, long id, bool isToUp)
        //{
        //    var authCategory = await _dbContext.AuthorizationCategories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        //    var affectedCategory = await _dbContext.AuthorizationCategories.FirstOrDefaultAsync(x => (isToUp ? x.Order == authCategory.Order - 1 : x.Order == authCategory.Order + 1), cancellationToken);

        //    authCategory.Order = isToUp ? authCategory.Order - 1 : authCategory.Order + 1;
        //    affectedCategory.Order = isToUp ? affectedCategory.Order + 1 : affectedCategory.Order - 1;
        //}
    }
}

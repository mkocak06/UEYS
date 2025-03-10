using Core.EkipModels;
using Core.Interfaces;
using Infrastructure.EkipData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EkipService : IEkipService
    {
        private readonly EkipDbContext _ekipContext;

        public EkipService(EkipDbContext ekipContext)
        {
            _ekipContext = ekipContext;
        }

        public async Task<T> GetByAsync<T>(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate) where T : class
        {
            return await _ekipContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<List<T>> GetListByAsync<T>(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate) where T : class
        {
            return await _ekipContext.Set<T>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }
    }
}

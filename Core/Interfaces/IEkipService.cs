using Core.EkipModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEkipService
    {
        Task<T> GetByAsync<T>(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate) where T : class;
        Task<List<T>> GetListByAsync<T>(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate) where T : class;    }
}

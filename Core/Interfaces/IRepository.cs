using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<T> GetByAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate);
        Task<List<T>> ListAsync(CancellationToken cancellationToken);
        Task<T> AddAsync(CancellationToken cancellationToken, T entity);
        T Add(T entity);
        Task AddRangeAsync(CancellationToken cancellationToken, IEnumerable<T> entities);
        void Update(T entity);
        void SoftDelete(T entity);
        void HardDelete(T entity);
        void HardDeleteRange(IEnumerable<T> entities);
        Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate = null);
        IQueryable<T> Get(Expression<Func<T, bool>> whereExpression);
        Task<T> GetIncluding(CancellationToken cancellationToken, Expression<Func<T, bool>> whereExpression,
            params Expression<Func<T, object>>[] includeProperties);
        Task<List<T>> GetIncludingList(CancellationToken cancellationToken, Expression<Func<T, bool>> whereExpression,
            params Expression<Func<T, object>>[] includeProperties);
        Task<bool> AnyAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate);
        IQueryable<T> Queryable();
        IQueryable<T> IncludingQueryable(Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] includeProperties);
    }
}

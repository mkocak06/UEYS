using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public EfRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById(long id)
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public Task<T> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            return _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public Task<T> GetByAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<List<T>> ListAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T> AddAsync(CancellationToken cancellationToken, T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

            return entity;
        }
        public async Task AddRangeAsync(CancellationToken cancellationToken, IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        }
        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);

            return entity;
        }
        public void Update(T entity)
        {
            var updateDateField = entity.GetType().GetProperty("UpdateDate");
            if (updateDateField != null)
            {
                updateDateField.SetValue(entity, DateTime.UtcNow);
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void SoftDelete(T entity)
        {
            var isDeletedField = entity.GetType().GetProperty("IsDeleted");
            if (isDeletedField != null)
            {
                isDeletedField.SetValue(entity, true);

                var deleteDateField = entity.GetType().GetProperty("DeleteDate");
                deleteDateField.SetValue(entity, DateTime.UtcNow);

                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            else
                _dbContext.Set<T>().Remove(entity);
        }
        public void HardDelete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
        public void HardDeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }
        public async Task<List<T>> GetAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate = null)
        {
            return predicate != null ? await _dbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken) :
                 await _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        public async Task<T> GetIncluding(CancellationToken cancellationToken, Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] includeProperties)
        {
            var queryable = _dbContext.Set<T>().AsSplitQuery().Where(whereExpression);

            foreach (var includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<T>> GetIncludingList(CancellationToken cancellationToken, Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] includeProperties)
        {
            var queryable = _dbContext.Set<T>().AsSplitQuery().Where(whereExpression);

            foreach (var includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            return await queryable.ToListAsync(cancellationToken);
        }
        public Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken);
        }
        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }
        public async Task<bool> AnyAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AnyAsync(predicate, cancellationToken);
        }
        public IQueryable<T> Queryable()
        {
            return _dbContext.Set<T>();
        }
        public IQueryable<T> IncludingQueryable(Expression<Func<T, bool>> whereExpression, params Expression<Func<T, object>>[] includeProperties)
        {
            var queryable = _dbContext.Set<T>().AsSplitQuery().AsNoTracking().Where(whereExpression).AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            return queryable;
        }
    }
}

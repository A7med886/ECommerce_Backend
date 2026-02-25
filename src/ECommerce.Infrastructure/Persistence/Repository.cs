using ECommerce.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Persistence
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetQueryable(bool tracking = true)
        {
            return tracking ? _dbSet.AsQueryable() : _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken cancellationToken = default)
        {
            var query = tracking ? _dbSet.AsQueryable() : _dbSet.AsNoTracking().AsQueryable();
            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
            //return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken cancellationToken = default)
        {
            var query = tracking ? _dbSet.AsQueryable() : _dbSet.AsNoTracking().AsQueryable();
            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
            //return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken cancellationToken = default)
        {
            var query = tracking ? _dbSet.AsQueryable() : _dbSet.AsNoTracking().AsQueryable();
            return await query.Where(predicate).ToListAsync(cancellationToken);
            //return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
        }
    }
}

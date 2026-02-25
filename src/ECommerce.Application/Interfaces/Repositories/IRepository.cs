using System.Linq.Expressions;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetQueryable(bool tracking = true);
        Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity, CancellationToken cancellationToken = default);
        void Delete(T entity, CancellationToken cancellationToken = default);
    }
}

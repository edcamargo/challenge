using System.Linq.Expressions;
using System.Threading;

namespace Domain.Inteface.Repositories;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Expose SaveChanges so callers can control transaction boundaries (Unit of Work)
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
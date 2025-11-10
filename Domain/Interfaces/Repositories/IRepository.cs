using System.Linq.Expressions;

namespace Domain.Inteface.Repositories;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(object id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // Expose SaveChanges so callers can control transaction boundaries (Unit of Work)
    Task<int> SaveChangesAsync();
}
using System.Linq.Expressions;
using Domain.Inteface.Repositories;
using InfraStructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Data.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DataContext _context;

    public Repository(DataContext context)
    {
        _context = context;
    }
    
    public async Task<TEntity?> GetByIdAsync(object id)
    {
        return await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        // SaveChanges removed to allow Unit of Work control by caller
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        // _context.Entry(entity).State = EntityState.Modified;
        
        _context.Set<TEntity>().Update(entity);
        
        // SaveChanges removed to allow Unit of Work control by caller
        return await Task.FromResult(entity);
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        // SaveChanges removed to allow Unit of Work control by caller
        return await Task.FromResult(true);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var result = await _context.Set<TEntity>().Where(predicate).ToListAsync();
        return result;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using InfraStructure.Data.Context;
using Domain.Entities;
using Domain.Intefaces.Repositories;

namespace InfraStructure.Data.Repositories;
public class TaskRepository : Repository<Tasks>, ITaskRepository
{
    private readonly DataContext _context;
    public TaskRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tasks>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.Set<Tasks>()
            .Include(t => t.User)
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }
}
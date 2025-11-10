using Domain.Entities;
using Domain.Inteface.Repositories;
using System.Threading;

namespace Domain.Intefaces.Repositories;

public interface ITaskRepository : IRepository<Tasks>
{
    // Add task-specific query methods here if needed in the future
    Task<IEnumerable<Tasks>> GetAllByUserIdAsync(Guid userId);
}

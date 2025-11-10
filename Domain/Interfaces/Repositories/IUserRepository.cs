using Domain.Entities;
using Domain.Inteface.Repositories;

namespace Domain.Intefaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    // Add user-specific query methods here if needed in the future
}

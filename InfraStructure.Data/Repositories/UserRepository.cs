using Domain.Entities;
using Domain.Intefaces.Repositories;
using InfraStructure.Data.Context;

namespace InfraStructure.Data.Repositories;
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext context) : base(context)
    { }
}

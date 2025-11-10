using Application.Dtos.User;
using Domain.Entities;
using Domain.Validations;

namespace Application.Services.Interfaces;

public interface IUserService
{
    Task<OperationResult<User>> Add(UserCreateDto dto, CancellationToken cancellationToken = default);
    Task<OperationResult<User>> Update(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default);
    Task<OperationResult<bool>> Delete(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<User?> GetById(Guid id, CancellationToken cancellationToken = default); 
}
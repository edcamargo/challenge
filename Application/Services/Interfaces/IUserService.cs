using Application.Dtos.User;
using Domain.Entities;
using Application.Common;

namespace Application.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<User>> Add(UserCreateDto dto);
    Task<ApiResponse<User>> Update(Guid id, UserUpdateDto dto);
    Task<ApiResponse<bool>> Delete(Guid id);
    Task<ApiResponse<IEnumerable<User>>> GetAll(int pageNumber, int pageSize);
    Task<ApiResponse<User>> GetById(Guid id); 
}
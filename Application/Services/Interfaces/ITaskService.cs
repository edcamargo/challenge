using Application.Common;
using Application.Dtos.Task;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface ITaskService
{
    Task<ApiResponse<Tasks>> Add(TaskCreateDto dto);
    Task<ApiResponse<Tasks>> Update(Guid id, TaskUpdateDto dto);
    Task<ApiResponse<bool>> Delete(Guid id);
    Task<ApiResponse<IEnumerable<Tasks>>> GetAll(int pageNumber, int pageSize);
    Task<ApiResponse<Tasks>> GetById(Guid id);
    Task<ApiResponse<IEnumerable<Tasks>>> GetAllByUserId(Guid userId);
}
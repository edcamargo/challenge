using Application.Common;
using Application.Dtos.Task;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Intefaces;
using Domain.Intefaces.Repositories;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public TaskService(ITaskRepository taskRepository, 
                       IUserRepository userRepository,
                       IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Tasks>> Add(TaskCreateDto dto)
    {
        var tasks = dto.ToEntity();
        
        // Validação da entidade
        var validation = tasks.EhValido();
        if (!validation.IsValid)
            return ApiResponse<Tasks>.ValidationFailure(validation);

        // Validação se o usuario existe
        var user = await _userRepository.GetByIdAsync(tasks.UserId);
        if (user is null)
            return ApiResponse<Tasks>.Error(400, "Usuário associado não encontrado.", "UserId");

        tasks.IsCompleted.Equals(true);
        var addedTask = await _taskRepository.AddAsync(tasks);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<Tasks>.Success(addedTask);
    }
    
    public async Task<ApiResponse<Tasks>> Update(Guid id, Guid taskId)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask is null)
            return ApiResponse<Tasks>.NotFound("Tarefa não encontrada.");

        var entity = await _taskRepository.GetByIdAsync(taskId);
        if (entity is null)
            return ApiResponse<Tasks>.NotFound("Task não foi encontrada.");

        // Validação da entidade
        var validation = entity.EhValido();
        if (!validation.IsValid)
            return ApiResponse<Tasks>.ValidationFailure(validation);
    
        entity.MarkCompleted();
        var updatedTask = await _taskRepository.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<Tasks>.Success(updatedTask);
    }

    public async Task<ApiResponse<Tasks>> GetById(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null)
            return ApiResponse<Tasks>.NotFound("Tarefa não encontrada.");

        return ApiResponse<Tasks>.Success(task);
    }
    
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask is null)
            return ApiResponse<bool>.NotFound("Tarefa não encontrada.");

        await _taskRepository.DeleteAsync(existingTask);
        var affected = await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Success(affected > 0);
    }
    
    public async Task<ApiResponse<IEnumerable<Tasks>>> GetAll(int pageNumber, int pageSize)
    {
        var tasks = await _taskRepository.GetAllAsync();
        return ApiResponse<IEnumerable<Tasks>>.Success(tasks);
    }

    public async Task<ApiResponse<IEnumerable<Tasks>>> GetAllByUserId(Guid userId)
    {
        var tasks = await _taskRepository.GetAllByUserIdAsync(userId);
        return ApiResponse<IEnumerable<Tasks>>.Success(tasks);
    }
}
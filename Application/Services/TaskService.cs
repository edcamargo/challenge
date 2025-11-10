using Application.Services.Interfaces;
using Domain.Intefaces.Repositories;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    
    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }
}
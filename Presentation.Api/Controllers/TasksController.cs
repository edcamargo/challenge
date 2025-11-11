using Application.Dtos.Task;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.Common;
using Presentation.Api.Extensions;

namespace Presentation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskResponseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetAllByUserId(Guid userId)
    {
        var response = await _taskService.GetAllByUserId(userId);
        return response.ToActionResult(TaskResponseDto.FromEntity);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _taskService.GetById(id);
        return response.ToActionResult(TaskResponseDto.FromEntity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
    {
        var response = await _taskService.Add(dto);
        return response.ToCreatedAtActionResult(
            nameof(GetById),
            TaskResponseDto.FromEntity,
            taskDto => new { id = taskDto.Id });
    }

    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(typeof(ApiResponse<TaskResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] Guid taskId)
    {
        var response = await _taskService.Update(id, taskId);
        return response.ToActionResult(TaskResponseDto.FromEntity);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _taskService.Delete(id);
        return response.ToNoContentResult();
    }
}

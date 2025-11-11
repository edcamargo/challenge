using Application.Common;
using Application.Dtos.Task;
using Application.Services.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Presentation.Api.Controllers;

namespace Challenge.Test.Unit.Presentation.Controllers
{
    public class TasksControllerTests
    {
        [Fact]
        public async Task GetAllByUserId_ReturnsOk_WithUserTasks()
        {
            var svc = Substitute.For<ITaskService>();
            var userId = Guid.NewGuid();
            var tasks = new List<Tasks>
            {
                Tasks.Create("Task U", "d", DateTime.UtcNow.AddDays(2), userId)
            };
            svc.GetAllByUserId(userId).Returns(Task.FromResult(ApiResponse<IEnumerable<Tasks>>.Success(tasks)));

            var controller = new TasksController(svc);

            var action = await controller.GetAllByUserId(userId);

            action.Should().BeOfType<OkObjectResult>();
            var ok = action as OkObjectResult;
            var body = ok!.Value as ApiResponse<IEnumerable<TaskResponseDto>>;
            body!.Data.Should().NotBeNull();
            body.Data!.First().UserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var svc = Substitute.For<ITaskService>();
            var userId = Guid.NewGuid();
            var task = Tasks.Create("TBY", "d", DateTime.UtcNow.AddDays(3), userId);
            svc.GetById(task.Id).Returns(Task.FromResult(ApiResponse<Tasks>.Success(task)));

            var controller = new TasksController(svc);

            var action = await controller.GetById(task.Id);

            action.Should().BeOfType<OkObjectResult>();
            var ok = action as OkObjectResult;
            var body = ok!.Value as ApiResponse<TaskResponseDto>;
            body!.Data.Should().NotBeNull();
            body.Data!.Id.Should().Be(task.Id);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenSuccess()
        {
            var svc = Substitute.For<ITaskService>();
            var userId = Guid.NewGuid();
            var dto = new TaskCreateDto("New", "d", DateTime.UtcNow, DateTime.UtcNow.AddDays(2), userId);
            var created = Tasks.Create(dto.Title, dto.Description, dto.DueDate, dto.UserId);
            svc.Add(Arg.Any<TaskCreateDto>()).Returns(Task.FromResult(ApiResponse<Tasks>.Success(created)));

            var controller = new TasksController(svc);

            var action = await controller.Create(dto);

            action.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = action as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(TasksController.GetById));
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            var svc = Substitute.For<ITaskService>();
            var userId = Guid.NewGuid();
            var task = Tasks.Create("TU", "d", DateTime.UtcNow.AddDays(4), userId);
            var updateDto = new TaskUpdateDto(task.Id, "TU", "d", task.CreatedAt, task.DueDate, task.UserId);
            svc.Update(task.Id, updateDto.Id).Returns(Task.FromResult(ApiResponse<Tasks>.Success(task)));

            var controller = new TasksController(svc);

            var action = await controller.Update(task.Id, updateDto.Id);

            action.Should().BeOfType<OkObjectResult>();
            var ok = action as OkObjectResult;
            var body = ok!.Value as ApiResponse<TaskResponseDto>;
            body!.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            var svc = Substitute.For<ITaskService>();
            var taskId = Guid.NewGuid();
            svc.Delete(taskId).Returns(Task.FromResult(ApiResponse<bool>.Success(true)));

            var controller = new TasksController(svc);

            var action = await controller.Delete(taskId);

            action.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenServiceReturnsError()
        {
            var svc = Substitute.For<ITaskService>();
            var id = Guid.NewGuid();
            svc.GetById(id).Returns(Task.FromResult(ApiResponse<Tasks>.Failure(new ApiError(404, "Not found"))));

            var controller = new TasksController(svc);

            var action = await controller.GetById(id);

            action.Should().BeOfType<ObjectResult>();
            var obj = action as ObjectResult;
            obj!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            var svc = Substitute.For<ITaskService>();
            var userId = Guid.NewGuid();
            var dto = new TaskCreateDto("", "d", DateTime.UtcNow, DateTime.UtcNow.AddDays(2), userId);
            svc.Add(Arg.Any<TaskCreateDto>()).Returns(Task.FromResult(ApiResponse<Tasks>.Failure(new ApiError(400, "Validation"))));

            var controller = new TasksController(svc);

            var action = await controller.Create(dto);

            action.Should().BeOfType<ObjectResult>();
            var obj = action as ObjectResult;
            obj!.StatusCode.Should().Be(400);
        }
    }
}


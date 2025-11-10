using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Presentation.Api.Dto;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Challenge.Test.Integration;

public class TasksIntegrationTests : IClassFixture<WebApplicationFactory<Presentation.Api.Program>>
{
    private readonly WebApplicationFactory<Presentation.Api.Program> _factory;

    public TasksIntegrationTests(WebApplicationFactory<Presentation.Api.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateUser_AndCreateTask_ReturnsCreated()
    {
        var client = _factory.CreateClient();

        // Create user
        var userReq = new { name = "Integration User", email = "integration@example.com" };
        var userResp = await client.PostAsJsonAsync("/api/users", userReq);
        userResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var userBody = await userResp.Content.ReadFromJsonAsync<ResponseEnvelope<UserDto>>();
        userBody.Should().NotBeNull();
        var userId = userBody!.Data!.Id;

        // Create task for user
        var taskReq = new
        {
            title = "Integration Task",
            description = "Integration test task",
            dueDate = DateTime.UtcNow.AddDays(2),
            userId = userId
        };

        var taskResp = await client.PostAsJsonAsync("/api/tasks", taskReq);
        taskResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var taskBody = await taskResp.Content.ReadFromJsonAsync<ResponseEnvelope<TaskDto>>();
        taskBody.Should().NotBeNull();
        taskBody!.Data!.Title.Should().Be("Integration Task");
    }
}

// Simple response envelope DTOs used by the API tests
public class ResponseEnvelope<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string[]? Notifications { get; set; }
}

public class UserDto { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } }
public class TaskDto { public Guid Id { get; set; } public string Title { get; set; } = string.Empty; public string? Description { get; set; } public DateTime CreatedAt { get; set; } public DateTime? DueDate { get; set; } public Guid UserId { get; set; } public bool IsCompleted { get; set; } }


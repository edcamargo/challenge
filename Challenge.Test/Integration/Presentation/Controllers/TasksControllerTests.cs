using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfraStructure.Data.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Presentation.Api;

namespace Challenge.Test.Integration.Presentation.Controllers
{
    public class TasksControllerTests
    {
        private WebApplicationFactory<Program> CreateFactoryWithInMemoryDb(string dbName)
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(dbName));

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                    ctx.Database.EnsureCreated();
                });
            });
        }

        [Fact]
        public async Task GetAll_Returns_Inserted_Tasks()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            // seed: need a user and a task
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = User.Create("Owner", "owner@example.com");
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();

                var task = Domain.Entities.Tasks.Create("T1", "desc", DateTime.UtcNow.AddDays(1), user.Id);
                ctx.Tasks.Add(task);
                await ctx.SaveChangesAsync();
            }

            var client = factory.CreateClient();
            var res = await client.GetAsync("/api/tasks");
            res.EnsureSuccessStatusCode();
            var apiResponse = await res.Content.ReadFromJsonAsync<Application.Common.ApiResponse<IEnumerable<Application.Dtos.Task.TaskResponseDto>>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Data.Should().NotBeNull();
            apiResponse.Data!.Any(t => t.Title == "T1" && t.Description == "desc").Should().BeTrue();
        }

        [Fact]
        public async Task GetAllByUserId_Returns_Tasks_With_UserInfo()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid userId;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = User.Create("Owner2", "owner2@example.com");
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();
                userId = user.Id;

                var task = Domain.Entities.Tasks.Create("T2", "desc2", DateTime.UtcNow.AddDays(2), user.Id);
                ctx.Tasks.Add(task);
                await ctx.SaveChangesAsync();
            }

            var client = factory.CreateClient();
            var res = await client.GetAsync($"/api/tasks/user/{userId}");
            res.EnsureSuccessStatusCode();
            var apiResponse = await res.Content.ReadFromJsonAsync<Application.Common.ApiResponse<IEnumerable<Application.Dtos.Task.TaskResponseDto>>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Data.Should().NotBeNull();
            apiResponse.Data!.Any(t => t.Title == "T2" && t.User != null && t.User.Email == "owner2@example.com").Should().BeTrue();
        }

        [Fact]
        public async Task Create_Creates_And_Returns_Task()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid userId;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = User.Create("Owner3", "owner3@example.com");
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();
                userId = user.Id;
            }

            var client = factory.CreateClient();
            var payload = new
            {
                title = "TaskNew",
                description = "t desc",
                createdAt = DateTime.UtcNow,
                dueDate = DateTime.UtcNow.AddDays(3),
                userId = userId,
                isCompleted = false
            };

            var res = await client.PostAsJsonAsync("/api/tasks", payload);
            res.StatusCode.Should().Be(HttpStatusCode.Created);
            var apiResponse = await res.Content.ReadFromJsonAsync<Application.Common.ApiResponse<Application.Dtos.Task.TaskResponseDto>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Data.Should().NotBeNull();
            apiResponse.Data!.Title.Should().Be("TaskNew");

            // verify persisted
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var exists = ctx.Tasks.Any(t => t.Title == "TaskNew");
                exists.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Create_Returns_400_When_User_NotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            var client = factory.CreateClient();
            var payload = new
            {
                title = "TaskInvalidUser",
                description = "desc",
                createdAt = DateTime.UtcNow,
                dueDate = DateTime.UtcNow.AddDays(3),
                userId = Guid.NewGuid(), // non-existing user
                isCompleted = false
            };

            var res = await client.PostAsJsonAsync("/api/tasks", payload);
            res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var apiResponse = await res.Content.ReadFromJsonAsync<Application.Common.ApiResponse<object>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Erros.Any(e => e.Message.Contains("Usuário associado não encontrado")).Should().BeTrue();
        }

        [Fact]
        public async Task Create_Returns_400_When_ValidationFails()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid userId;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = User.Create("Uval", "uval@example.com");
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();
                userId = user.Id;
            }

            var client = factory.CreateClient();
            var payload = new
            {
                title = "", // invalid: empty
                description = "desc",
                createdAt = DateTime.UtcNow,
                dueDate = DateTime.UtcNow.AddDays(3),
                userId = userId,
                isCompleted = false
            };

            var res = await client.PostAsJsonAsync("/api/tasks", payload);
            res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var apiResponse = await res.Content.ReadFromJsonAsync<Application.Common.ApiResponse<object>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Erros.Any(e => e.Message.Contains("O título da tarefa é obrigatório")).Should().BeTrue();
        }

        [Fact]
        public async Task Update_Updates_Task_Status()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid taskId;
            Guid userId;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = User.Create("Owner4", "owner4@example.com");
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();

                userId = user.Id;

                var task = Domain.Entities.Tasks.Create("T3", "desc3", DateTime.UtcNow.AddDays(4), user.Id);
                ctx.Tasks.Add(task);
                await ctx.SaveChangesAsync();
                taskId = task.Id;
            }

            var client = factory.CreateClient();
            var updatePayload = new { title = "T3", description = "desc3", createdAt = DateTime.UtcNow, dueDate = DateTime.UtcNow.AddDays(4), userId = userId, isCompleted = true };
            var putRes = await client.PutAsJsonAsync($"/api/tasks/{taskId}/complete", updatePayload);
            putRes.EnsureSuccessStatusCode();
            var apiResponse = await putRes.Content.ReadFromJsonAsync<Application.Common.ApiResponse<Application.Dtos.Task.TaskResponseDto>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Data.Should().NotBeNull();
            apiResponse.Data!.IsCompleted.Should().BeTrue();

            // verify persisted
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var updated = ctx.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == taskId);
                updated.Should().NotBeNull();
                updated!.IsCompleted.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Update_Returns_404_When_Task_NotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            var client = factory.CreateClient();
            var updatePayload = new { title = "NoTask", description = "d", createdAt = DateTime.UtcNow, dueDate = DateTime.UtcNow.AddDays(1), userId = Guid.NewGuid(), isCompleted = true };
            var putRes = await client.PutAsJsonAsync($"/api/tasks/{Guid.NewGuid()}/complete", updatePayload);
            putRes.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var apiResponse = await putRes.Content.ReadFromJsonAsync<Application.Common.ApiResponse<object>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Erros.Any(e => e.Message.Contains("Tarefa não encontrada")).Should().BeTrue();
        }

        [Fact]
        public async Task Delete_Removes_Task_And_GetById_Returns_NotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid taskId;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = User.Create("Owner5", "owner5@example.com");
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();

                var task = Domain.Entities.Tasks.Create("T4", "desc4", DateTime.UtcNow.AddDays(5), user.Id);
                ctx.Tasks.Add(task);
                await ctx.SaveChangesAsync();
                taskId = task.Id;
            }

            var client = factory.CreateClient();
            var delRes = await client.DeleteAsync($"/api/tasks/{taskId}");
            delRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getRes = await client.GetAsync($"/api/tasks/{taskId}");
            getRes.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_Returns_404_When_Task_NotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            var client = factory.CreateClient();
            var delRes = await client.DeleteAsync($"/api/tasks/{Guid.NewGuid()}");
            delRes.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var apiResponse = await delRes.Content.ReadFromJsonAsync<Application.Common.ApiResponse<object>>();
            apiResponse.Should().NotBeNull();
            apiResponse!.Erros.Any(e => e.Message.Contains("Tarefa não encontrada")).Should().BeTrue();
        }
    }
}

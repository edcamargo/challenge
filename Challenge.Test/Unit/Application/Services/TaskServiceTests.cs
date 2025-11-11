using Application.Dtos.Task;
using Application.Services;
using Domain.Entities;
using Domain.Intefaces.Repositories;
using Domain.Intefaces;
using FluentAssertions;
using NSubstitute;

namespace Challenge.Test.Unit.Application.Services
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task Add_ReturnsSuccess_WhenValid()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();

            var user = User.Create("U", "u@example.com");
            userRepo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<User?>(user));

            var dto = new TaskCreateDto("TaskTitle", "d", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), user.Id);
            var entity = dto.ToEntity();

            // sanity check: ensure entity validation passes in test
            var val = entity.EhValido();
            val.IsValid.Should().BeTrue(string.Join(";", val.Errors.Select(e => e.ErrorMessage)));

            taskRepo.AddAsync(Arg.Any<Tasks>()).Returns(Task.FromResult(entity));
            uow.SaveChangesAsync().Returns(Task.FromResult(1));

            var svc = new TaskService(taskRepo, userRepo, uow);

            var res = await svc.Add(dto);

            res.Should().NotBeNull();
            res.Data.Should().NotBeNull();
            res.Erros.Should().BeEmpty();
            res.Data!.Title.Should().Be("TaskTitle");
        }

        [Fact]
        public async Task Add_ReturnsBadRequest_WhenUserNotFound()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();

            userRepo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<User?>(null));

            var dto = new TaskCreateDto("T", "d", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
            var svc = new TaskService(taskRepo, userRepo, uow);

            var res = await svc.Add(dto);

            res.Should().NotBeNull();
            res.Erros.Should().NotBeEmpty();
            res.Erros[0].StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Add_ReturnsValidationFailure_WhenInvalid()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();

            var user = User.Create("U2", "u2@example.com");
            userRepo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<User?>(user));

            var dto = new TaskCreateDto("", "d", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), user.Id); // invalid title
            var svc = new TaskService(taskRepo, userRepo, uow);

            var res = await svc.Add(dto);

            res.Should().NotBeNull();
            res.Erros.Should().NotBeEmpty();
            res.Erros[0].StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenTaskMissing()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            taskRepo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Tasks?>(null));

            var svc = new TaskService(taskRepo, userRepo, uow);
            var res = await svc.Update(Guid.NewGuid(), Guid.NewGuid());

            res.Should().NotBeNull();
            res.Erros.Should().NotBeEmpty();
            res.Erros[0].StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            taskRepo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Tasks?>(null));

            var svc = new TaskService(taskRepo, userRepo, uow);
            var res = await svc.Delete(Guid.NewGuid());

            res.Should().NotBeNull();
            res.Erros.Should().NotBeEmpty();
            res.Erros[0].StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess_WhenDeleted()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();

            var task = Tasks.Create("TD", "d", DateTime.UtcNow.AddDays(1), Guid.NewGuid());
            taskRepo.GetByIdAsync(task.Id).Returns(Task.FromResult<Tasks?>(task));
            taskRepo.DeleteAsync(task).Returns(Task.FromResult(true));
            uow.SaveChangesAsync().Returns(Task.FromResult(1));

            var svc = new TaskService(taskRepo, userRepo, uow);
            var res = await svc.Delete(task.Id);

            res.Should().NotBeNull();
            res.Erros.Should().BeEmpty();
            res.Data.Should().BeTrue();
        }

        [Fact]
        public async Task GetAll_Returns_AllTasks()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();

            var list = new[] { Tasks.Create("A", "a", DateTime.UtcNow.AddDays(1), Guid.NewGuid()) };
            taskRepo.GetAllAsync().Returns(Task.FromResult<IEnumerable<Tasks>>(list));

            var svc = new TaskService(taskRepo, userRepo, uow);
            var res = await svc.GetAll(1, 10);

            res.Should().NotBeNull();
            res.Data.Should().NotBeNull();
            res.Data!.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllByUserId_Returns_UserTasks()
        {
            var taskRepo = Substitute.For<ITaskRepository>();
            var userRepo = Substitute.For<IUserRepository>();
            var uow = Substitute.For<IUnitOfWork>();

            var userId = Guid.NewGuid();
            var list = new[] { Tasks.Create("UA", "a", DateTime.UtcNow.AddDays(1), userId) };
            taskRepo.GetAllByUserIdAsync(userId).Returns(Task.FromResult<IEnumerable<Tasks>>(list));

            var svc = new TaskService(taskRepo, userRepo, uow);
            var res = await svc.GetAllByUserId(userId);

            res.Should().NotBeNull();
            res.Data.Should().NotBeNull();
            res.Data!.First().UserId.Should().Be(userId);
        }
    }
}

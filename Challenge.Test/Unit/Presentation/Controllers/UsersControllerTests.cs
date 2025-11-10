using Application.Common;
using Application.Dtos.User;
using Application.Services.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Presentation.Api.Controllers;

namespace Challenge.Test.Unit.Presentation.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk_WithMappedUsers()
        {
            var svc = Substitute.For<IUserService>();
            var users = new[] { User.Create("U1", "u1@example.com") };
            svc.GetAll(Arg.Any<int>(), Arg.Any<int>()).Returns(Task.FromResult(ApiResponse<IEnumerable<User>>.Success(users)));

            var controller = new UsersController(svc);
            var action = await controller.GetAll();

            action.Should().BeOfType<OkObjectResult>();
            var ok = action as OkObjectResult;
            var body = ok!.Value as ApiResponse<IEnumerable<UserReponseDto>>;
            body!.Data.Should().NotBeNull();
            body.Data!.First().Name.Should().Be("U1");
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var svc = Substitute.For<IUserService>();
            var user = User.Create("UB", "ub@example.com");
            svc.GetById(user.Id).Returns(Task.FromResult(ApiResponse<User>.Success(user)));

            var controller = new UsersController(svc);
            var action = await controller.GetById(user.Id);

            action.Should().BeOfType<OkObjectResult>();
            var ok = action as OkObjectResult;
            var body = ok!.Value as ApiResponse<UserReponseDto>;
            body!.Data.Should().NotBeNull();
            body.Data!.Email.Should().Be("ub@example.com");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var svc = Substitute.For<IUserService>();
            svc.GetById(Arg.Any<Guid>()).Returns(Task.FromResult(ApiResponse<User>.Failure(new ApiError(404, "Not found"))));

            var controller = new UsersController(svc);
            var action = await controller.GetById(Guid.NewGuid());

            action.Should().BeOfType<ObjectResult>();
            var obj = action as ObjectResult;
            obj!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenSuccess()
        {
            var svc = Substitute.For<IUserService>();
            var user = User.Create("NC", "nc@example.com");
            svc.Add(Arg.Any<UserCreateDto>()).Returns(Task.FromResult(ApiResponse<User>.Success(user)));

            var controller = new UsersController(svc);
            var dto = new UserCreateDto("NC", "nc@example.com");
            var action = await controller.Create(dto);

            action.Should().BeOfType<CreatedAtActionResult>();
            var created = action as CreatedAtActionResult;
            created!.ActionName.Should().Be(nameof(UsersController.GetById));
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            var svc = Substitute.For<IUserService>();
            svc.Add(Arg.Any<UserCreateDto>()).Returns(Task.FromResult(ApiResponse<User>.Failure(new ApiError(400, "Invalid"))));

            var controller = new UsersController(svc);
            var dto = new UserCreateDto("", "");
            var action = await controller.Create(dto);

            action.Should().BeOfType<ObjectResult>();
            var obj = action as ObjectResult;
            obj!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            var svc = Substitute.For<IUserService>();
            var user = User.Create("UP", "up@example.com");
            svc.Update(user.Id, Arg.Any<UserUpdateDto>()).Returns(Task.FromResult(ApiResponse<User>.Success(user)));

            var controller = new UsersController(svc);
            var dto = new UserUpdateDto(user.Id, "UP", "up@example.com");
            var action = await controller.Update(user.Id, dto);

            action.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            var svc = Substitute.For<IUserService>();
            svc.Delete(Arg.Any<Guid>()).Returns(Task.FromResult(ApiResponse<bool>.Success(true)));

            var controller = new UsersController(svc);
            var action = await controller.Delete(Guid.NewGuid());

            action.Should().BeOfType<NoContentResult>();
        }
    }
}


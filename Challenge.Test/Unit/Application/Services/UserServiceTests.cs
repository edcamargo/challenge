using Application.Dtos.User;
using Application.Services;
using Domain.Entities;
using Domain.Intefaces;
using Domain.Intefaces.Repositories;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Challenge.Test.Unit.Application.Services;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new UserService(_userRepository, _unitOfWork);
    }

    #region Add Tests

    [Fact]
    public async Task Add_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new UserCreateDto("John Doe", "john@example.com");
        var user = new User("John Doe", new Email("john@example.com"));

        _userRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(new List<User>());
        _userRepository.AddAsync(Arg.Any<User>()).Returns(user);
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _sut.Add(dto);

        // Assert
        result.Data.Should().NotBeNull();
        result.Erros.Should().BeEmpty();
        await _userRepository.Received(1).AddAsync(Arg.Any<User>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Add_WithInvalidEmail_ShouldReturnValidationFailure()
    {
        // Arrange
        var dto = new UserCreateDto("John Doe", "invalid-email");

        // Act
        var result = await _sut.Add(dto);

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.Key != null && e.Key.Contains("Email"));
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Add_WithEmptyName_ShouldReturnValidationFailure()
    {
        // Arrange
        var dto = new UserCreateDto("", "john@example.com");

        // Act
        var result = await _sut.Add(dto);

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.Key == "Name");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Add_WithDuplicateEmail_ShouldReturnError()
    {
        // Arrange
        var dto = new UserCreateDto("John Doe", "john@example.com");
        var existingUser = new User("Jane Doe", new Email("john@example.com"));

        _userRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(new List<User> { existingUser });

        // Act
        var result = await _sut.Add(dto);

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.Message == "E-mail já cadastrado." && e.Key == "Email");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new UserUpdateDto(userId, "John Updated", "john.updated@example.com");
        var existingUser = new User("John Doe", new Email("john@example.com")) { Id = userId };
        var updatedUser = new User("John Updated", new Email("john.updated@example.com")) { Id = userId };

        _userRepository.GetByIdAsync(userId).Returns(existingUser);
        _userRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(new List<User>());
        _userRepository.UpdateAsync(Arg.Any<User>()).Returns(updatedUser);
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _sut.Update(userId, dto);

        // Assert
        result.Data.Should().NotBeNull();
        result.Erros.Should().BeEmpty();
        await _userRepository.Received(1).UpdateAsync(Arg.Any<User>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Update_WithNonExistingUser_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new UserUpdateDto(userId, "John Doe", "john@example.com");

        _userRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act
        var result = await _sut.Update(userId, dto);

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.StatusCode == 404);
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Update_WithInvalidEmail_ShouldReturnValidationFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new UserUpdateDto(userId, "John Doe", "invalid-email");
        var existingUser = new User("John Doe", new Email("john@example.com")) { Id = userId };

        _userRepository.GetByIdAsync(userId).Returns(existingUser);

        // Act
        var result = await _sut.Update(userId, dto);

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.Key != null && e.Key.Contains("Email"));
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Update_WithDuplicateEmail_ShouldReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new UserUpdateDto(userId, "John Doe", "jane@example.com");
        var existingUser = new User("John Doe", new Email("john@example.com")) { Id = userId };
        var anotherUser = new User("Jane Doe", new Email("jane@example.com"));

        _userRepository.GetByIdAsync(userId).Returns(existingUser);
        _userRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(new List<User> { anotherUser });

        // Act
        var result = await _sut.Update(userId, dto);

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.Message == "E-mail já cadastrado." && e.Key == "Email");
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithExistingUser_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("John Doe", new Email("john@example.com")) { Id = userId };

        _userRepository.GetByIdAsync(userId).Returns(user);

        // Act
        var result = await _sut.GetById(userId);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(userId);
        result.Erros.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_WithNonExistingUser_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act
        var result = await _sut.GetById(userId);

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.StatusCode == 404);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithExistingUser_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("John Doe", new Email("john@example.com")) { Id = userId };

        _userRepository.GetByIdAsync(userId).Returns(user);
        _userRepository.DeleteAsync(user).Returns(true);
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _sut.Delete(userId);

        // Assert
        result.Data.Should().BeTrue();
        result.Erros.Should().BeEmpty();
        await _userRepository.Received(1).DeleteAsync(user);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_WithNonExistingUser_ShouldReturnNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act
        var result = await _sut.Delete(userId);

        // Assert
        result.Data.Should().BeFalse();
        result.Erros.Should().NotBeEmpty();
        result.Erros.Should().Contain(e => e.StatusCode == 404);
        await _userRepository.DidNotReceive().DeleteAsync(Arg.Any<User>());
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User("John Doe", new Email("john@example.com")),
            new User("Jane Doe", new Email("jane@example.com"))
        };

        _userRepository.GetAllAsync().Returns(users);

        // Act
        var result = await _sut.GetAll(1, 100);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Erros.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_WithNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        _userRepository.GetAllAsync().Returns(new List<User>());

        // Act
        var result = await _sut.GetAll(1, 100);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Erros.Should().BeEmpty();
    }

    #endregion
}


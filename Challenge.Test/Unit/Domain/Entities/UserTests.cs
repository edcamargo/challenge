using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Challenge.Test.Unit.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var name = "John Doe";
        var email = new Email("john@example.com");

        // Act
        var user = new User(name, email);

        // Assert
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EhValido_WithValidUser_ShouldReturnValidResult()
    {
        // Arrange
        var user = new User("John Doe", new Email("john@example.com"));

        // Act
        var result = user.EhValido();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void EhValido_WithInvalidName_ShouldReturnInvalidResult(string invalidName)
    {
        // Arrange
        var user = new User(invalidName, new Email("john@example.com"));

        // Act
        var result = user.EhValido();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Id_ShouldBeSettable()
    {
        // Arrange
        var user = new User("John Doe", new Email("john@example.com"));
        var newId = Guid.NewGuid();

        // Act
        user.Id = newId;

        // Assert
        user.Id.Should().Be(newId);
    }
}


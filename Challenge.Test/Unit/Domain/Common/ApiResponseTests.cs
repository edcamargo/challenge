using Application.Common;
using FluentAssertions;
using FluentValidation.Results;

namespace Challenge.Test.Unit.Domain.Common;

public class ApiResponseTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResponse()
    {
        // Arrange
        var data = "test data";

        // Act
        var response = ApiResponse<string>.Success(data);

        // Assert
        response.Data.Should().Be(data);
        response.Erros.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithApiErrors_ShouldCreateFailureResponse()
    {
        // Arrange
        var errors = new[]
        {
            new ApiError(400, "Error 1", "field1"),
            new ApiError(400, "Error 2", "field2")
        };

        // Act
        var response = ApiResponse<string>.Failure(errors);

        // Assert
        response.Data.Should().BeNull();
        response.Erros.Should().HaveCount(2);
        response.Erros.Should().Contain(e => e.Message == "Error 1");
        response.Erros.Should().Contain(e => e.Message == "Error 2");
    }

    [Fact]
    public void ValidationFailure_WithValidationResult_ShouldCreateFailureResponse()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        });

        // Act
        var response = ApiResponse<string>.ValidationFailure(validationResult);

        // Assert
        response.Data.Should().BeNull();
        response.Erros.Should().HaveCount(2);
        response.Erros.Should().Contain(e => e.Message == "Name is required" && e.Key == "Name");
        response.Erros.Should().Contain(e => e.Message == "Email is invalid" && e.Key == "Email");
        response.Erros.Should().OnlyContain(e => e.StatusCode == 400);
    }

    [Fact]
    public void ValidationFailure_WithCustomStatusCode_ShouldCreateFailureResponseWithCustomStatus()
    {
        // Arrange
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Field", "Error")
        });

        // Act
        var response = ApiResponse<string>.ValidationFailure(validationResult, 422);

        // Assert
        response.Erros.Should().OnlyContain(e => e.StatusCode == 422);
    }

    [Fact]
    public void NotFound_ShouldCreateNotFoundResponse()
    {
        // Act
        var response = ApiResponse<string>.NotFound("User not found");

        // Assert
        response.Data.Should().BeNull();
        response.Erros.Should().HaveCount(1);
        response.Erros.First().StatusCode.Should().Be(404);
        response.Erros.First().Message.Should().Be("User not found");
        response.Erros.First().Key.Should().Be("id");
    }

    [Fact]
    public void NotFound_WithCustomKey_ShouldCreateNotFoundResponseWithCustomKey()
    {
        // Act
        var response = ApiResponse<string>.NotFound("Resource not found", "resourceId");

        // Assert
        response.Erros.First().Key.Should().Be("resourceId");
    }

    [Fact]
    public void Error_ShouldCreateErrorResponse()
    {
        // Act
        var response = ApiResponse<string>.Error(400, "Bad request", "field");

        // Assert
        response.Data.Should().BeNull();
        response.Erros.Should().HaveCount(1);
        response.Erros.First().StatusCode.Should().Be(400);
        response.Erros.First().Message.Should().Be("Bad request");
        response.Erros.First().Key.Should().Be("field");
    }

    [Fact]
    public void Error_WithoutKey_ShouldCreateErrorResponseWithEmptyKey()
    {
        // Act
        var response = ApiResponse<string>.Error(500, "Internal error");

        // Assert
        response.Erros.First().Key.Should().Be("");
    }
}


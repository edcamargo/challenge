using Application.Common;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Extensions;

namespace Challenge.Test.Unit.Presentation.Extensions;

public class ApiResponseExtensionsTests
{
    #region HasErrors Tests

    [Fact]
    public void HasErrors_WithErrors_ShouldReturnTrue()
    {
        // Arrange
        var response = ApiResponse<string>.Failure(new ApiError(400, "Error"));

        // Act
        var result = response.HasErrors();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasErrors_WithoutErrors_ShouldReturnFalse()
    {
        // Arrange
        var response = ApiResponse<string>.Success("data");

        // Act
        var result = response.HasErrors();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ToActionResult Tests

    [Fact]
    public void ToActionResult_WithSuccess_ShouldReturnOkResult()
    {
        // Arrange
        var response = ApiResponse<string>.Success("test data");

        // Act
        var result = response.ToActionResult();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public void ToActionResult_WithError_ShouldReturnErrorResult()
    {
        // Arrange
        var response = ApiResponse<string>.Failure(new ApiError(400, "Bad request"));

        // Act
        var result = response.ToActionResult();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public void ToActionResult_WithNotFoundError_ShouldReturn404()
    {
        // Arrange
        var response = ApiResponse<string>.Failure(new ApiError(404, "Not found"));

        // Act
        var result = response.ToActionResult();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(404);
    }

    #endregion

    #region ToCreatedAtActionResult Tests

    [Fact]
    public void ToCreatedAtActionResult_WithSuccess_ShouldReturnCreatedResult()
    {
        // Arrange
        var data = new { Id = Guid.NewGuid(), Name = "Test" };
        var response = ApiResponse<object>.Success(data);

        // Act
        var result = response.ToCreatedAtActionResult("GetById", dto => new { id = ((dynamic)dto).Id });

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be("GetById");
    }

    [Fact]
    public void ToCreatedAtActionResult_WithError_ShouldReturnErrorResult()
    {
        // Arrange
        var response = ApiResponse<string>.Failure(new ApiError(400, "Bad request"));

        // Act
        var result = response.ToCreatedAtActionResult("GetById");

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(400);
    }

    #endregion

    #region ToNoContentResult Tests

    [Fact]
    public void ToNoContentResult_WithSuccess_ShouldReturnNoContentResult()
    {
        // Arrange
        var response = ApiResponse<bool>.Success(true);

        // Act
        var result = response.ToNoContentResult();

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToNoContentResult_WithError_ShouldReturnErrorResult()
    {
        // Arrange
        var response = ApiResponse<bool>.Failure(new ApiError(404, "Not found"));

        // Act
        var result = response.ToNoContentResult();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(404);
    }

    #endregion

    #region Map Tests

    [Fact]
    public void Map_WithSuccessAndValidMapper_ShouldTransformData()
    {
        // Arrange
        var user = new User("John Doe", new Email("john@example.com"));
        var response = ApiResponse<User>.Success(user);

        // Act
        var result = response.Map(u => new { u.Name, Email = u.Email.Endereco });

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("John Doe");
        result.Data.Email.Should().Be("john@example.com");
        result.Erros.Should().BeEmpty();
    }

    [Fact]
    public void Map_WithError_ShouldPropagateError()
    {
        // Arrange
        var error = new ApiError(400, "Error message");
        var response = ApiResponse<User>.Failure(error);

        // Act
        var result = response.Map(u => new { u.Name });

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().HaveCount(1);
        result.Erros.First().Message.Should().Be("Error message");
    }

    [Fact]
    public void Map_WithNullData_ShouldReturnError()
    {
        // Arrange
        var response = new ApiResponse<string>
        {
            Data = null,
            Erros = new List<ApiError>()
        };

        // Act
        var result = response.Map(s => s.ToUpper());

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().HaveCount(1);
        result.Erros.First().StatusCode.Should().Be(500);
        result.Erros.First().Message.Should().Contain("nulos");
    }

    #endregion

    #region MapCollection Tests

    [Fact]
    public void MapCollection_WithSuccessAndValidMapper_ShouldTransformCollection()
    {
        // Arrange
        var users = new List<User>
        {
            new User("John Doe", new Email("john@example.com")),
            new User("Jane Doe", new Email("jane@example.com"))
        };
        var response = ApiResponse<IEnumerable<User>>.Success(users);

        // Act
        var result = response.MapCollection(u => new { u.Name, Email = u.Email.Endereco });

        // Assert
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data!.First().Name.Should().Be("John Doe");
        result.Erros.Should().BeEmpty();
    }

    [Fact]
    public void MapCollection_WithError_ShouldPropagateError()
    {
        // Arrange
        var error = new ApiError(400, "Error message");
        var response = ApiResponse<IEnumerable<User>>.Failure(error);

        // Act
        var result = response.MapCollection(u => new { u.Name });

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().HaveCount(1);
        result.Erros.First().Message.Should().Be("Error message");
    }

    [Fact]
    public void MapCollection_WithNullData_ShouldReturnError()
    {
        // Arrange
        var response = new ApiResponse<IEnumerable<string>>
        {
            Data = null,
            Erros = new List<ApiError>()
        };

        // Act
        var result = response.MapCollection(s => s.ToUpper());

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().HaveCount(1);
        result.Erros.First().StatusCode.Should().Be(500);
    }

    #endregion

    #region MapAsync Tests

    [Fact]
    public async Task MapAsync_WithSuccess_ShouldTransformData()
    {
        // Arrange
        var user = new User("John Doe", new Email("john@example.com"));
        var responseTask = Task.FromResult(ApiResponse<User>.Success(user));

        // Act
        var result = await responseTask.MapAsync(u => new { u.Name });

        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("John Doe");
        result.Erros.Should().BeEmpty();
    }

    [Fact]
    public async Task MapAsync_WithError_ShouldPropagateError()
    {
        // Arrange
        var error = new ApiError(400, "Error");
        var responseTask = Task.FromResult(ApiResponse<User>.Failure(error));

        // Act
        var result = await responseTask.MapAsync(u => new { u.Name });

        // Assert
        result.Data.Should().BeNull();
        result.Erros.Should().HaveCount(1);
    }

    #endregion

    #region MapCollectionAsync Tests

    [Fact]
    public async Task MapCollectionAsync_WithSuccess_ShouldTransformCollection()
    {
        // Arrange
        var users = new List<User> { new User("John", new Email("john@example.com")) };
        var responseTask = Task.FromResult(ApiResponse<IEnumerable<User>>.Success(users));

        // Act
        var result = await responseTask.MapCollectionAsync(u => new { u.Name });

        // Assert
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Erros.Should().BeEmpty();
    }

    #endregion

    #region ToActionResultAsync Tests

    [Fact]
    public async Task ToActionResultAsync_WithSuccess_ShouldReturnOkResult()
    {
        // Arrange
        var responseTask = Task.FromResult(ApiResponse<string>.Success("data"));

        // Act
        var result = await responseTask.ToActionResultAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region ToCreatedAtActionResultAsync Tests

    [Fact]
    public async Task ToCreatedAtActionResultAsync_WithSuccess_ShouldReturnCreatedResult()
    {
        // Arrange
        var data = new { Id = Guid.NewGuid() };
        var responseTask = Task.FromResult(ApiResponse<object>.Success(data));

        // Act
        var result = await responseTask.ToCreatedAtActionResultAsync("GetById", dto => new { id = ((dynamic)dto).Id });

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    #endregion

    #region ToNoContentResultAsync Tests

    [Fact]
    public async Task ToNoContentResultAsync_WithSuccess_ShouldReturnNoContentResult()
    {
        // Arrange
        var responseTask = Task.FromResult(ApiResponse<bool>.Success(true));

        // Act
        var result = await responseTask.ToNoContentResultAsync();

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    #endregion

    #region Additional ToActionResult/ToCreatedAtActionResult Tests

    [Fact]
    public void ToActionResult_WithMapperAndNullData_Returns500()
    {
        // Arrange
        var response = new ApiResponse<User> { Data = null, Erros = new List<ApiError>() };

        // Act
        var result = response.ToActionResult<User, object>(u => new { u.Name });

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(500);
        var body = obj.Value as ApiResponse<object>;
        body!.Erros.First().Message.Should().Contain("nulos");
    }

    [Fact]
    public void ToActionResult_CollectionMapper_WithEmptyCollection_ReturnsOkWithEmpty()
    {
        // Arrange
        var response = ApiResponse<IEnumerable<User>>.Success(new List<User>());

        // Act
        var result = response.ToActionResult<User, object>(u => new { u.Name });

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        var body = ok!.Value as ApiResponse<IEnumerable<object>>;
        body!.Data.Should().NotBeNull();
        body.Data.Should().BeEmpty();
    }

    [Fact]
    public void ToCreatedAtActionResult_WithMapperAndRouteFactory_ReturnsCreated()
    {
        // Arrange
        var user = new User("Created", new Email("c@example.com"));
        var response = ApiResponse<User>.Success(user);

        // Act
        var result = response.ToCreatedAtActionResult<User, object>("Get", u => new { u.Name }, o => new { id = ((dynamic)o).Name });

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var created = result as CreatedAtActionResult;
        created!.ActionName.Should().Be("Get");
    }

    [Fact]
    public void ToCreatedAtActionResult_WithMapperAndNullData_Returns500()
    {
        // Arrange
        var response = new ApiResponse<User>() { Data = null, Erros = new List<ApiError>() };

        // Act
        var result = response.ToCreatedAtActionResult<User, object>("Get", u => new { u.Name });

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(500);
    }

    [Fact]
    public void ToActionResult_CollectionMapper_WithOneItem_ReturnsOkAndMappedItem()
    {
        // Arrange
        var users = new List<User> { new User("Solo", new Email("solo@example.com")) };
        var response = ApiResponse<IEnumerable<User>>.Success(users);

        // Act
        var result = response.ToActionResult<User, object>(u => new { u.Name, Email = u.Email.Endereco });

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        var body = ok!.Value as ApiResponse<IEnumerable<object>>;
        body!.Data.Should().NotBeNull();
        body.Data.Should().HaveCount(1);
        body.Data!.First().Should().NotBeNull();
    }

    [Fact]
    public void ToActionResult_CollectionMapper_WithNullData_Returns500()
    {
        // Arrange
        var response = new ApiResponse<IEnumerable<User>> { Data = null, Erros = new List<ApiError>() };

        // Act
        var result = response.ToActionResult<User, object>(u => new { u.Name });

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(500);
        var body = obj.Value as ApiResponse<IEnumerable<object>>;
        body!.Erros.First().Message.Should().Contain("nulos");
    }

    [Fact]
    public void ToActionResult_CollectionMapper_WithError_ReturnsErrorStatus()
    {
        // Arrange
        var response = ApiResponse<IEnumerable<User>>.Failure(new ApiError(418, "I'm a teapot"));

        // Act
        var result = response.ToActionResult<User, object>(u => new { u.Name });

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(418);
    }

    #endregion
}

using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Presentation.Api.Middlewares;

namespace Challenge.Test.Unit.Presentation.Middlewares;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenInnerThrows_WritesInternalServerErrorJson()
    {
        // Arrange
        RequestDelegate throwingDelegate = ctx => throw new System.InvalidOperationException("Boom");
        var middleware = new ExceptionMiddleware(throwingDelegate);

        var context = new DefaultHttpContext();
        var memStream = new MemoryStream();
        context.Response.Body = memStream;

        var logger = NullLogger<ExceptionMiddleware>.Instance;

        // Act
        await middleware.InvokeAsync(context, logger);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        memStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memStream, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        body.Should().Contain("\"success\":false");
        body.Should().Contain("Boom");
    }

    [Fact]
    public async Task InvokeAsync_WhenInnerThrowsWithInnerException_IncludesInnerMessage()
    {
        // Arrange
        RequestDelegate throwingDelegate = ctx => throw new System.Exception("Outer", new System.Exception("Inner"));
        var middleware = new ExceptionMiddleware(throwingDelegate);

        var context = new DefaultHttpContext();
        var memStream = new MemoryStream();
        context.Response.Body = memStream;

        var logger = NullLogger<ExceptionMiddleware>.Instance;

        // Act
        await middleware.InvokeAsync(context, logger);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        memStream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(memStream, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        body.Should().Contain("Outer");
        body.Should().Contain("Inner");
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextAndDoesNotModifyResponse()
    {
        // Arrange
        RequestDelegate next = ctx =>
        {
            ctx.Response.StatusCode = 202;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionMiddleware(next);
        var context = new DefaultHttpContext();
        var memStream = new MemoryStream();
        context.Response.Body = memStream;

        var logger = NullLogger<ExceptionMiddleware>.Instance;

        // Act
        await middleware.InvokeAsync(context, logger);

        // Assert
        context.Response.StatusCode.Should().Be(202);
        memStream.Length.Should().Be(0);
    }
}

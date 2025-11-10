using System.Net;
using Newtonsoft.Json;

namespace Presentation.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            logger.LogError(ex, "Erro no back end of application.");

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                success = false,
                errors = new List<string> { ex.Message + (ex.InnerException?.Message ?? string.Empty) }
            }));
        }
    }
}
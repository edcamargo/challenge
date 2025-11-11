using Microsoft.AspNetCore.Rewrite;
using Presentation.Api.Middlewares;

namespace Presentation.Api.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UsePresentationMiddlewares(this WebApplication app)
    {
        // Exception middleware - catch unhandled exceptions from all subsequent middleware
        app.UseMiddleware<ExceptionMiddleware>();

        // Routing must come before authentication/authorization when using endpoint routing
        app.UseRouting();

        // CORS
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        // Authentication & Authorization
        //app.UseAuthentication();
        //app.UseAuthorization();

        // Swagger UI and JSON
        app.UseSwaggerDocumentation();

        // Redirect root to swagger
        var option = new RewriteOptions();
        option.AddRedirect("^$", "swagger");
        app.UseRewriter(option);

        // Map controllers/endpoints
        app.MapControllers();

        return app;
    }
}
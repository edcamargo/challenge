using InfraStructure.Data.Context;
using InfraStructure.Ioc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // API Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        // ApiExplorer that supports versioning (used by Swagger to create docs per version)
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; // e.g. v1, v1.0
            options.SubstituteApiVersionInUrl = true;
        });

        // Registrar infraestrutura do domínio (repositories, etc.)
        services.AddInfrastructure(configuration);

        // Banco em memória para desenvolvimento/testes
        services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

        // Registrar CORS (policy inline can still be used in pipeline)
        services.AddCors();

        return services;
    } 
}
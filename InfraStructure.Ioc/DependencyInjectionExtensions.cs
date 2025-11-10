using Application.Services;
using Application.Services.Interfaces;
using Domain.Intefaces.Repositories;
using InfraStructure.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfraStructure.Ioc;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddServices()
            .AddRepository();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
        
        // Register cross-cutting services
        //services.AddScoped<IEncryptions, Encryptions>();
        
        // Password hasher (BCrypt)
        //services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        return services;
    }

    private static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<Domain.Intefaces.IUnitOfWork, InfraStructure.Data.UnitOfWork>();

        return services;
    }
}
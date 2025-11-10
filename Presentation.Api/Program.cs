using Microsoft.EntityFrameworkCore;
using InfraStructure.Data.Context;
using InfraStructure.Data.Repositories;
using Microsoft.OpenApi.Models;
using Presentation.Api.Extensions;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// InMemory DbContext for demo/testing
builder.Services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("TodoDb"));

// Registrar serviços da apresentação (controllers, dbcontext, infra, CORS)
builder.Services.AddPresentationServices(builder.Configuration);

// Registrar Swagger
builder.Services.AddSwaggerDocumentation();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Middleware pipeline Error global handling, routing, CORS, auth, swagger, rewrite, map controllers
app.UsePresentationMiddlewares();

app.Run();
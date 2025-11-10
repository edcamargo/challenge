using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfraStructure.Data.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Presentation.Api;

namespace Challenge.Test.Integration.Presentation.Controllers
{
    public class UsersControllerTests
    {
        private WebApplicationFactory<Program> CreateFactoryWithInMemoryDb(string dbName)
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing DataContext registration
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    // Register a new DataContext using a unique in-memory database for the test
                    services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(dbName));

                    // Ensure DB is created
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                    ctx.Database.EnsureCreated();
                });
            });
        }

        [Fact]
        public async Task GetAll_Returns_Inserted_Users()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            // seed
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var user = User.Create("TesteGetAll", "getall@example.com");
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();
            }

            var client = factory.CreateClient();
            var res = await client.GetAsync("/api/users");
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            body.Should().Contain("TesteGetAll");
            body.Should().Contain("getall@example.com");
        }

        [Fact]
        public async Task GetById_Returns_User_When_Found()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid id;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                id = Guid.NewGuid();
                var user = User.Create("TesteById", "byid@example.com");
                // override id to predictable value
                user.Id = id;
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();
            }

            var client = factory.CreateClient();
            var res = await client.GetAsync($"/api/users/{id}");
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            body.Should().Contain("TesteById");
            body.Should().Contain("byid@example.com");
        }

        [Fact]
        public async Task Create_Creates_And_Returns_User()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            var client = factory.CreateClient();
            var payload = new { name = "NovoUser", email = "novo@example.com" };
            var res = await client.PostAsJsonAsync("/api/users", payload);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            body.Should().Contain("NovoUser");
            body.Should().Contain("novo@example.com");

            // verify persisted
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var exists = ctx.User.Any(u => u.Name == "NovoUser");
                exists.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Update_Updates_And_Returns_User()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid id;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                id = Guid.NewGuid();
                var user = User.Create("Antes", "antes@example.com");
                user.Id = id;
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();
            }

            var client = factory.CreateClient();
            var updatePayload = new { name = "Depois", email = "depois@example.com" };
            var putRes = await client.PutAsJsonAsync($"/api/users/{id}", updatePayload);
            putRes.EnsureSuccessStatusCode();
            var putBody = await putRes.Content.ReadAsStringAsync();
            putBody.Should().Contain("Depois");
            putBody.Should().Contain("depois@example.com");

            // verify persisted
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                var updated = ctx.User.AsNoTracking().FirstOrDefault(u => u.Id == id);
                updated.Should().NotBeNull();
                updated!.Name.Should().Be("Depois");
            }
        }

        [Fact]
        public async Task Delete_Removes_User_And_GetById_Returns_NotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            using var factory = CreateFactoryWithInMemoryDb(dbName);

            Guid id;
            using (var scope = factory.Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
                id = Guid.NewGuid();
                var user = User.Create("ParaDeletar", "del@example.com");
                user.Id = id;
                ctx.User.Add(user);
                await ctx.SaveChangesAsync();
            }

            var client = factory.CreateClient();
            var delRes = await client.DeleteAsync($"/api/users/{id}");
            delRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getRes = await client.GetAsync($"/api/users/{id}");
            getRes.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

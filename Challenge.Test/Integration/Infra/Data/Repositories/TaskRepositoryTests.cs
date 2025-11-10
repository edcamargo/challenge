using FluentAssertions;
using InfraStructure.Data.Context;
using InfraStructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Challenge.Test.Integration.Infra.Data.Repositories
{
    public class TaskRepositoryTests
    {
        private DataContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new DataContext(options);
        }

        [Fact]
        public async Task AddAsync_And_GetByIdAsync_Work()
        {
            var dbName = Guid.NewGuid().ToString();
            using var ctx = CreateContext(dbName);

            var user = User.Create("RepoUser", "repo@example.com");
            ctx.User.Add(user);
            await ctx.SaveChangesAsync();

            var repo = new TaskRepository(ctx);
            var task = Tasks.Create("RepoTask", "rdesc", DateTime.UtcNow.AddDays(2), user.Id);
            await repo.AddAsync(task);
            await repo.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(task.Id);
            fetched.Should().NotBeNull();
            fetched!.Title.Should().Be("RepoTask");
        }

        [Fact]
        public async Task GetAllByUserIdAsync_Returns_UserTasks()
        {
            var dbName = Guid.NewGuid().ToString();
            using var ctx = CreateContext(dbName);

            var user = User.Create("U2", "u2.repo@example.com");
            ctx.User.Add(user);
            await ctx.SaveChangesAsync();

            var repo = new TaskRepository(ctx);
            var t1 = Tasks.Create("T1", "a", DateTime.UtcNow.AddDays(1), user.Id);
            var t2 = Tasks.Create("T2", "b", DateTime.UtcNow.AddDays(2), user.Id);
            await repo.AddAsync(t1);
            await repo.AddAsync(t2);
            await repo.SaveChangesAsync();

            var list = await repo.GetAllByUserIdAsync(user.Id);
            list.Should().HaveCount(2);
            list.Select(t => t.Title).Should().Contain(new[] { "T1", "T2" });
        }

        [Fact]
        public async Task DeleteAsync_Removes_Entity()
        {
            var dbName = Guid.NewGuid().ToString();
            using var ctx = CreateContext(dbName);

            var user = User.Create("U3", "u3.repo@example.com");
            ctx.User.Add(user);
            await ctx.SaveChangesAsync();

            var repo = new TaskRepository(ctx);
            var t = Tasks.Create("ToDelete", "x", DateTime.UtcNow.AddDays(1), user.Id);
            await repo.AddAsync(t);
            await repo.SaveChangesAsync();

            var existing = await repo.GetByIdAsync(t.Id);
            existing.Should().NotBeNull();

            // Delete the original tracked instance to avoid EF duplicate tracking when removing
            var deleted = await repo.DeleteAsync(t);
            deleted.Should().BeTrue();
            await repo.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(t.Id);
            fetched.Should().BeNull();
        }
    }
}

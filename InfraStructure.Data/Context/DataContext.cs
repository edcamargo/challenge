using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Data.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    { }

    #region DbSet
    
    public DbSet<User> User { get; set; }
    public DbSet<Tasks> Tasks { get; set; }

    #endregion DbSet

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        // Map Email value object as owned type
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Name).IsRequired();

            b.OwnsOne(u => u.Email, eb =>
            {
                // Remove HasColumnName because it's an extension from relational providers.
                eb.Property(e => e.Endereco).IsRequired();
            });
        });
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }
}
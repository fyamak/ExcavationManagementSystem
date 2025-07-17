using Infrastructure.Data.Postgres.Entities;
using Infrastructure.Data.Postgres.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgres.EntityFramework;

public class PostgresContext : DbContext
{
    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
        modelBuilder.ApplyConfiguration(new IncomeConfiguration());
        modelBuilder.ApplyConfiguration(new JobConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserTokenConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Customer>  Customers   => Set<Customer>();
    public DbSet<Expense>   Expenses    => Set<Expense>();
    public DbSet<Income>    Incomes     => Set<Income>();
    public DbSet<Job>       Jobs        => Set<Job>();
    public DbSet<User>      Users       => Set<User>();
    public DbSet<UserToken> UserTokens  => Set<UserToken>();
    public DbSet<Vehicle>   Vehicles    => Set<Vehicle>();
}

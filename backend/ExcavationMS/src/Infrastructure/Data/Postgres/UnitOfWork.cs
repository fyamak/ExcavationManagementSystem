using Infrastructure.Data.Postgres.Entities.Base.Interface;
using Infrastructure.Data.Postgres.EntityFramework;
using Infrastructure.Data.Postgres.Repositories;
using Infrastructure.Data.Postgres.Repositories.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Event;

namespace Infrastructure.Data.Postgres;

public class UnitOfWork : IUnitOfWork
{
    private readonly PostgresContext _postgresContext;
    private readonly IMediator       _mediator;

    private CustomerRepository? _customerRepository;
    private ExpenseRepository _expenseRepository;
    private IncomeRepository _incomeRepository;
    private JobRepository _jobRepository;
    private UserRepository?      _userRepository;
    private UserTokenRepository? _userTokenRepository;
    private VehicleRepository _vehicleRepository;

    public ICustomerRepository Customers => _customerRepository ??= new CustomerRepository(_postgresContext);
    public IExpenseRepository Expenses => _expenseRepository ??= new ExpenseRepository(_postgresContext);
    public IIncomeRepository Incomes => _incomeRepository ??= new IncomeRepository(_postgresContext);
    public IJobRepository Jobs => _jobRepository ??= new JobRepository(_postgresContext);
    public IUserRepository      Users      => _userRepository ??= new UserRepository(_postgresContext);
    public IUserTokenRepository UserTokens => _userTokenRepository ??= new UserTokenRepository(_postgresContext);
    public IVehicleRepository Vehicles => _vehicleRepository ??= new VehicleRepository(_postgresContext);

    public UnitOfWork(PostgresContext postgresContext, IMediator mediator)
    {
        _postgresContext = postgresContext;
        _mediator        = mediator;
    }

    public async Task<int> CommitAsync()
    {
        var updatedEntities = _postgresContext.ChangeTracker.Entries<ITrackedEntity>()
            .Where(e => e.State == EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var updatedEntity in updatedEntities)
        {
            updatedEntity.UpdatedAt = DateTime.UtcNow;
        }

        var result = await _postgresContext.SaveChangesAsync();

        if (result > 0)
        {
            var entitiesWithEvents = _postgresContext.ChangeTracker.Entries<EntityWithEvents>()
                .Select(e => e.Entity).ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.GetEventsToPublish();

                foreach (var @event in events)
                {
                    await _mediator.Publish(@event);
                }
            }
        }

        return result;
    }

    public void Dispose()
    {
        _postgresContext.Dispose();
    }
}

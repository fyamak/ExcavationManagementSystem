using Infrastructure.Data.Postgres.Repositories.Interface;

namespace Infrastructure.Data.Postgres;

public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    IExpenseRepository Expenses { get; }
    IIncomeRepository Incomes { get; }
    IJobRepository Jobs { get; }
    IUserRepository      Users      { get; }
    IUserTokenRepository UserTokens { get; }
    IVehicleRepository Vehicles { get; }
    Task<int>            CommitAsync();
}

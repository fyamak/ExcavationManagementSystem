using Infrastructure.Data.Postgres.Entities;
using Infrastructure.Data.Postgres.EntityFramework;
using Infrastructure.Data.Postgres.Repositories.Base;
using Infrastructure.Data.Postgres.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgres.Repositories;

public class CustomerRepository : TrackedEntityRepository<Customer, int>, ICustomerRepository
{
    public CustomerRepository(PostgresContext postgresContext) : base(postgresContext)
    {
    }


    public async Task<(IList<Customer> items, int totalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? search, 
        bool includeDeleted = false, 
        bool tracked = false)
    {
        var query = PostgresContext.Customers.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search));
        }

        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);

    }

}

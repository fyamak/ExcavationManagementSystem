using Infrastructure.Data.Postgres.Entities;
using Infrastructure.Data.Postgres.Repositories.Base.Interface;

namespace Infrastructure.Data.Postgres.Repositories.Interface;

public interface ICustomerRepository : ITrackedEntityRepository<Customer,int>
{
    public Task<(IList<Customer> items, int totalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? search = null, 
        bool includeDeleted = false, 
        bool tracked = false);

}

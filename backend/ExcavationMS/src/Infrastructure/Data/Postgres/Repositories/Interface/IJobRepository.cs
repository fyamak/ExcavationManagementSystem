using Infrastructure.Data.Postgres.Entities;
using Infrastructure.Data.Postgres.Repositories.Base.Interface;

namespace Infrastructure.Data.Postgres.Repositories.Interface;

public interface IJobRepository : ITrackedEntityRepository<Job, int>
{
    public Task<(IList<Job> items, int totalCount)> GetPagedAsync(
        int vehicleId,
        int pageNumber,
        int pageSize,
        string? search,
        bool includeDeleted = false,
        bool tracked = false);

    public Task<Job> GetJobByIdAsync(int id);
}

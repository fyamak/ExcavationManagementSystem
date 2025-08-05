using Infrastructure.Data.Postgres.Entities;
using Infrastructure.Data.Postgres.EntityFramework;
using Infrastructure.Data.Postgres.Repositories.Base;
using Infrastructure.Data.Postgres.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Results;

namespace Infrastructure.Data.Postgres.Repositories;

public class JobRepository : TrackedEntityRepository<Job, int>, IJobRepository
{
    public JobRepository(PostgresContext postgresContext) : base(postgresContext)
    {
    }

    public async Task<(IList<Job> items, int totalCount)> GetPagedAsync(
        int vehicleId,
        int pageNumber,
        int pageSize,
        string? search,
        bool includeDeleted = false,
        bool tracked = false)
    {
        var query = PostgresContext.Jobs.AsQueryable();
        query = query.Where(x => x.VehicleId == vehicleId)
            .Include(c => c.Customer)
            .Include(e => e.Expenses)
            .Include(i => i.Incomes);


        if (!includeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(search));
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

    public async Task<Job> GetJobByIdAsync(int id)
    {
        return await PostgresContext.Jobs
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Incomes)
            .Include(x => x.Expenses)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}

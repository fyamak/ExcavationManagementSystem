using Infrastructure.Data.Postgres.Entities.Base;

namespace Infrastructure.Data.Postgres.Entities;

public class Customer : TrackedBaseEntity<int>
{
    public string Name { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}

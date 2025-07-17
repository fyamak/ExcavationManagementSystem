using Infrastructure.Data.Postgres.Entities.Base;

namespace Infrastructure.Data.Postgres.Entities;

public class Income : TrackedBaseEntity<int>
{
    public string Title { get; set; }
    public int Price { get; set; }
    public DateTime? Date { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
}

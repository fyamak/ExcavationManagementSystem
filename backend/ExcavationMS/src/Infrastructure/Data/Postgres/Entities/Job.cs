using Infrastructure.Data.Postgres.Entities.Base;

namespace Infrastructure.Data.Postgres.Entities;

public class Job : TrackedBaseEntity<int>
{
    public string Title { get; set; }
    public JobStatus Status { get; set; } // Enum: Planned, Ongoing, Completed
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? AgreementAmount { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }

    // One Customer has many Jobs
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // One Job has many Incomes and Expenses
    public ICollection<Income> Incomes { get; } = new List<Income>();
    public ICollection<Expense> Expenses { get; } = new List<Expense>();

    // Many to Many
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}

public enum JobStatus
{
    Planned,
    Continue,
    Completed
}

using Infrastructure.Data.Postgres.Entities.Base;

namespace Infrastructure.Data.Postgres.Entities;

public class Job : TrackedBaseEntity<int>
{
    public string Title { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int? AgreementAmount { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }

    // One Vehicle has many Jobs
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }

    // One Customer has many Jobs
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // One Job has many Incomes and Expenses
    public ICollection<Income> Incomes { get; } = new List<Income>();
    public ICollection<Expense> Expenses { get; } = new List<Expense>();

}

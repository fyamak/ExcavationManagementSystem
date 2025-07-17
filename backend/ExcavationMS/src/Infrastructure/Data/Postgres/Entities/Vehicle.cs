using Infrastructure.Data.Postgres.Entities.Base;

namespace Infrastructure.Data.Postgres.Entities;

public class Vehicle : TrackedBaseEntity<int>
{
    public string Title { get; set; }
    public VehicleType VehicleType { get; set; }
    public ICollection<Job> Jobs { get; set; } = new List<Job>();

}

public enum VehicleType
{
    Truck,
    Excavator
}

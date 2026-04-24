using Ardalis.Specification;
using Inventory.Domain.Common;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate;

public interface IVehicleRepository : IRepository<Vehicle>
{
    public Task<IReadOnlyList<Vehicle>> ListAsync(Specification<Vehicle> spec);
    public Task<int> CountAsync(Specification<Vehicle> spec);
}

using Inventory.Domain.Common;
using Inventory.Domain.Specifications.Vehicles;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate;

public interface IVehicleRepository : IRepository<Vehicle>
{
    public Task<IReadOnlyList<Vehicle>> ListAsync(VehicleListPaginatedSpecification spec);
    public Task<IReadOnlyList<Vehicle>> ListAsync(VehicleFilterPaginatedSpecification spec);
    public Task<int> CountAsync(VehicleListCountSpecification spec);
    public Task<int> CountAsync(VehicleFilterCountSpecification spec);
}

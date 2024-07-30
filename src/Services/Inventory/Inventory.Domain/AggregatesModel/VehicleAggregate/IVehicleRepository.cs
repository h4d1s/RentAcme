using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.Common;
using System.Linq.Expressions;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate;

public interface IVehicleRepository : IRepository<Vehicle>
{
}

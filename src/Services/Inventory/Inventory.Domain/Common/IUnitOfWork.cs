using Inventory.Domain.AggregatesModel.BookingAggregate;
using Inventory.Domain.AggregatesModel.BrandAggregate;
using Inventory.Domain.AggregatesModel.ModelAggregate;
using Inventory.Domain.AggregatesModel.VariantAggreate;
using Inventory.Domain.AggregatesModel.VehicleAggregate;

namespace Inventory.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    IVehicleRepository VehicleRepository { get; }
    IBookingRepository BookingRepository { get; }
    IBrandRepository BrandRepository { get; }
    IModelRepository ModelRepository { get; }
    IVariantRepository VariantRepository { get; }
    Task<int> SaveEntitiesAsync(CancellationToken cancellationToken);
}

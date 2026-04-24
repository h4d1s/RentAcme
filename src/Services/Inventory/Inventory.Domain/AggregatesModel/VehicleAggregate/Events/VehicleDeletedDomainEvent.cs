using MediatR;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate.Events;

public class VehicleDeletedDomainEvent : INotification
{
    public Guid VehicleId { get; private set; }

    public VehicleDeletedDomainEvent(Guid vehicleId)
    {
        VehicleId = vehicleId;
    }
}

using MediatR;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate.Events;

public class VehicleCreatedDomainEvent : INotification
{
    public Vehicle Vehicle { get; private set; }

    public VehicleCreatedDomainEvent(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }
}

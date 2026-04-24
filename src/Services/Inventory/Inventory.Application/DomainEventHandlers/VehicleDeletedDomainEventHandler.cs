using Inventory.Domain.AggregatesModel.VehicleAggregate.Events;
using MediatR;

namespace Inventory.Application.DomainEventHandlers;

public class VehicleDeletedDomainEventHandler : INotificationHandler<VehicleDeletedDomainEvent>
{
    public Task Handle(VehicleDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        // throw new NotImplementedException();
        return Task.CompletedTask;
    }
}

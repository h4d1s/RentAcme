using Inventory.Domain.AggregatesModel.VehicleAggregate.Events;
using MediatR;

namespace Inventory.Application.DomainEventHandlers;

public class VehicleCreatedDomainEventHandler : INotificationHandler<VehicleCreatedDomainEvent>
{
    public Task Handle(VehicleCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }
}

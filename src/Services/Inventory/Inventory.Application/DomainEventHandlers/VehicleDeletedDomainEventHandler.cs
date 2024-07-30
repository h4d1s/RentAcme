using Inventory.Domain.AggregatesModel.VehicleAggregate.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DomainEventHandlers;

public class VehicleDeletedDomainEventHandler : INotificationHandler<VehicleDeletedDomainEvent>
{
    public Task Handle(VehicleDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        // throw new NotImplementedException();
        return Task.CompletedTask;
    }
}

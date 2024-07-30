using Inventory.Domain.AggregatesModel.VehicleAggregate.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DomainEventHandlers;

public class VehicleCreatedDomainEventHandler : INotificationHandler<VehicleCreatedDomainEvent>
{
    public Task Handle(VehicleCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }
}

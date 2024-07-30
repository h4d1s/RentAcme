using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate.Events;

public class VehicleCreatedDomainEvent : INotification
{
    public Vehicle Vehicle { get; private set; }

    public VehicleCreatedDomainEvent(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }
}

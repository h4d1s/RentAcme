using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.AggregatesModel.VehicleAggregate.Events;

public class VehicleDeletedDomainEvent : INotification
{
    public Guid VehicleId { get; private set; }

    public VehicleDeletedDomainEvent(Guid vehicleId)
    {
        VehicleId = vehicleId;
    }
}

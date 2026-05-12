using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Events;

public class VehicleUnlockedIntegrationEvent
{
    public Guid BookingId { get; set; } = Guid.Empty;
    public Guid VehicleId { get; set; } = Guid.Empty;
}

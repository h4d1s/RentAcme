using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events;

public class BookingCanceledIntegrationEvent
{
    public Guid BookingId { get; set; }
    public Guid VehicleId { get; set; }
}

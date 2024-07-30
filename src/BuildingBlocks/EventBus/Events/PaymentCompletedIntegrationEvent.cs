using EventBus.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events;

public class PaymentCompletedIntegrationEvent : IPaymentCompletedIntegrationEvent
{
    public Guid CorrelationId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

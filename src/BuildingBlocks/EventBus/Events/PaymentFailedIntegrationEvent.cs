using EventBus.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events;

public class PaymentFailedIntegrationEvent : IPaymentFailedIntegrationEvent
{
    public Guid CorrelationId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

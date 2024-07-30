using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.Interfaces;

public interface IPaymentCompletedIntegrationEvent : CorrelatedBy<Guid>
{
    public string CustomerId { get; set; }
}

using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.Interfaces;

public interface IPaymentFailedIntegrationEvent : CorrelatedBy<Guid>
{
    public string ErrorMessage { get; set; }
    public Guid UserId { get; set; }
}

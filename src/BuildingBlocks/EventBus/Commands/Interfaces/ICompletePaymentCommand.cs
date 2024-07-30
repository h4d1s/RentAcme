using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Commands.Interfaces;

public interface ICompletePaymentCommand : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
}

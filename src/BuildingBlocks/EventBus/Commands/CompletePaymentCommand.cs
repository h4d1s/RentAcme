using EventBus.Commands.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Commands;

public class CompletePaymentCommand : ICompletePaymentCommand
{
    public Guid CorrelationId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
}

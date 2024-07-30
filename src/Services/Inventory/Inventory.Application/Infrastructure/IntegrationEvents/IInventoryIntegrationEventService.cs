using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Infrastructure.IntegrationEvents;

public interface IInventoryIntegrationEventService
{
    Task SendAsync<T>(T msg, string queueName) where T : class;
    Task PublishAsync<T>(T evt) where T : class;
}

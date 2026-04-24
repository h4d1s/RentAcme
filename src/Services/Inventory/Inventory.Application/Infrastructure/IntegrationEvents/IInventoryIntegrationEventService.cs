namespace Inventory.Application.Infrastructure.IntegrationEvents;

public interface IInventoryIntegrationEventService
{
    Task SendAsync<T>(T msg, string queueName) where T : class;
    Task PublishAsync<T>(T evt) where T : class;
}

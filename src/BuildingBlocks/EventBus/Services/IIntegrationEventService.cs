namespace EventBus.Services;

public interface IIntegrationEventService
{
    Task SendAsync<T>(T msg, string queueName) where T : class;
    Task PublishAsync<T>(T evt) where T : class;
}

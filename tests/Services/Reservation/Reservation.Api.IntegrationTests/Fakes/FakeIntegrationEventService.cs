using EventBus.Services;

namespace Reservation.API.IntegrationTests.Fakes;

public class FakeIntegrationEventService : IIntegrationEventService
{
    public Task PublishAsync<T>(T evt) where T : class
    {
        return Task.CompletedTask;
    }

    public Task SendAsync<T>(T msg, string queueName) where T : class
    {
        return Task.CompletedTask;
    }
}

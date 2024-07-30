using MassTransit;

namespace EventBus.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;

    public IntegrationEventService(
        ISendEndpointProvider sendEndpointProvider,
        IPublishEndpoint publishEndpoint)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(T evt) where T : class
    {
        await _publishEndpoint.Publish(evt);
    }

    public async Task SendAsync<T>(T msg, string queueName) where T : class
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));

        await sendEndpoint.Send(msg);
    }
}

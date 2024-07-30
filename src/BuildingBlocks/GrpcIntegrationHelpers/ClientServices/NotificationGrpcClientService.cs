using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notificationproto;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Channels;
using ConsulIntegrationHelpers.Services;

namespace GrpcIntegrationHelpers.ClientServices;

public class NotificationGrpcClientService : INotificationGrpcClientService, IDisposable
{
    private readonly IConsulServiceDiscovery _consulServiceDiscovery;
    private GrpcChannel _channel = null!;
    private NotificationProtoService.NotificationProtoServiceClient _client = null!;

    public NotificationGrpcClientService(IConsulServiceDiscovery consulServiceDiscovery)
    {
        _consulServiceDiscovery = consulServiceDiscovery;
        _ = CreateConnectionAsync();
    }

    public async Task SendEmailAsync(string recipient, string subject, string message)
    {
        if (string.IsNullOrEmpty(recipient))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Recipient is required."));
        }

        if (string.IsNullOrEmpty(subject))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Subject is required."));
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Message is required."));
        }

        await EnsureConnectionCreated();

        var request = new NotificationEmailRequest
        {
            Recipient = recipient,
            Subject = subject,
            Message = message,
        };
        await _client.SendEmailAsync(request);
    }

    private async Task EnsureConnectionCreated()
    {
        if (_client == null)
        {
            await CreateConnectionAsync();
            if (_client == null)
            {
                throw new Exception($"{nameof(NotificationProtoService.NotificationProtoServiceClient)} initialization failed.");
            }
        }
    }

    private async Task CreateConnectionAsync()
    {
        var address = await _consulServiceDiscovery.GetServiceAddress("notification");
        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _channel = GrpcChannel.ForAddress("https://" + address, new GrpcChannelOptions { HttpHandler = httpHandler });
        _client = new NotificationProtoService.NotificationProtoServiceClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}

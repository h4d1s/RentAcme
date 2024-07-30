using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Grpc.Core;
using Grpc.Net.Client;
using Userproto;
using GrpcIntegrationHelpers.Models;
using ConsulIntegrationHelpers.Services;

namespace GrpcIntegrationHelpers.ClientServices;

public class UserGrpcClientService : IUserGrpcClientService, IDisposable
{
    private readonly IConsulServiceDiscovery _consulServiceDiscovery;
    private GrpcChannel _channel = null!;
    private UserProtoService.UserProtoServiceClient _client = null!;

    public UserGrpcClientService(IConsulServiceDiscovery consulServiceDiscovery)
    {
        _consulServiceDiscovery = consulServiceDiscovery;
        _ = CreateConnectionAsync();
    }

    public async Task<bool> CheckIfExistsAsync(Guid id)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id is required."));
        }

        await EnsureConnectionCreated();

        var request = new CheckUserRequest { UserId = id.ToString() };
        var reply = await _client.CheckUserAsync(request);

        return reply.IsExists;
    }

    public async Task<UserDto> GetUserAsync(Guid id)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id is required."));
        }

        await EnsureConnectionCreated();

        var request = new GetUserRequest { UserId = id.ToString() };
        var reply = await _client.GetUserAsync(request);

        var user = new UserDto
        {
            Id = Guid.Parse(reply.Id),
            Email = reply.Email,
            FirstName = reply.FirstName,
            LastName = reply.LastName,
        };

        return user;
    }

    private async Task EnsureConnectionCreated()
    {
        if (_client == null)
        {
            await CreateConnectionAsync();
            if (_client == null)
            {
                throw new Exception($"{nameof(UserProtoService.UserProtoServiceClient)} initialization failed.");
            }
        }
    }

    private async Task CreateConnectionAsync()
    {
        var address = await _consulServiceDiscovery.GetServiceAddress("user");

        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _channel = GrpcChannel.ForAddress("https://" + address, new GrpcChannelOptions { HttpHandler = httpHandler });
        _client = new UserProtoService.UserProtoServiceClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}

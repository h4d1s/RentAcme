using Grpc.Core;
using Grpc.Net.Client;
using GrpcIntegrationHelpers.Models;
using Identity.Services;
using Userproto;

namespace GrpcIntegrationHelpers.ClientServices;

public class UserGrpcClientService : IUserGrpcClientService, IDisposable
{
    private readonly string _address = "https://user-api:8081";
    private readonly IIdentityTokenService _identityTokenService;
    private GrpcChannel _channel = null!;
    private UserProtoService.UserProtoServiceClient _client = null!;

    public UserGrpcClientService(
        IIdentityTokenService identityTokenService)
    {
        _identityTokenService = identityTokenService;
        CreateConnectionAsync();
    }

    public async Task<bool> CheckIfExistsAsync(Guid id)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id is required."));
        }

        EnsureConnectionCreated();

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

        EnsureConnectionCreated();

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

    public async Task<UserDto> GetUserByExternalIdAsync(string externalId)
    {
        if (string.IsNullOrEmpty(externalId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"{externalId} is required."));
        }

        EnsureConnectionCreated();

        var request = new GetUserByExternalIdRequest { UserExternalId = externalId };
        var reply = await _client.GetUserByExternalIdAsync(request);

        var user = new UserDto
        {
            Id = Guid.Parse(reply.Id),
            ExternalId = reply.ExternalId,
            Email = reply.Email,
            FirstName = reply.FirstName,
            LastName = reply.LastName,
        };

        return user;
    }

    private void EnsureConnectionCreated()
    {
        if (_client == null)
        {
            CreateConnectionAsync();
            if (_client == null)
            {
                throw new Exception($"{nameof(UserProtoService.UserProtoServiceClient)} initialization failed.");
            }
        }
    }

    private void CreateConnectionAsync()
    {
        var callCredentials = CallCredentials.FromInterceptor(async (context, metadata) =>
        {
            var token = await _identityTokenService.GetValidTokenAsync() ?? throw new ArgumentNullException("Token is not available.");
            metadata.Add("Authorization", $"Bearer {token}");
        });

        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _channel = GrpcChannel.ForAddress(_address, new GrpcChannelOptions
        {
            HttpHandler = httpHandler,
            Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials)
        });
        _client = new UserProtoService.UserProtoServiceClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}

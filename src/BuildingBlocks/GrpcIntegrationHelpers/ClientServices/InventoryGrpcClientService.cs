using ConsulIntegrationHelpers.Services;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcIntegrationHelpers.Models;
using Inventoryproto;

namespace GrpcIntegrationHelpers.ClientServices;

public class InventoryGrpcClientService : IInventoryGrpcClientService, IDisposable
{
    private readonly IConsulServiceDiscovery _consulServiceDiscovery;
    private GrpcChannel _channel = null!;
    private InventoryProtoService.InventoryProtoServiceClient _client = null!;

    public InventoryGrpcClientService(
        IConsulServiceDiscovery consulServiceDiscovery)
    {
        _consulServiceDiscovery = consulServiceDiscovery;
        _ = CreateConnectionAsync();
    }

    public async Task<VehicleDto> GetVehicleAsync(Guid id)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id is required."));
        }

        await EnsureConnectionCreated();

        var request = new GetVehicleRequest { VehicleId = id.ToString() };
        var reply = await _client.GetVehicleAsync(request);

        var response = new VehicleDto
        {
            RentalPricePerDay = (decimal)reply.RentalPricePerDay,
            RegistrationPlates = reply.RegistrationPlates,
            VariantId = Guid.Parse(reply.VariantId),
        };

        return response;
    }

    public async Task<bool> CheckIfExistsAsync(Guid id)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id is required."));
        }

        await EnsureConnectionCreated();

        var request = new CheckVehicleRequest { VehicleId = id.ToString() };
        var reply = await _client.CheckVehicleAsync(request);

        return reply.IsExists;
    }

    private async Task EnsureConnectionCreated()
    {
        if (_client == null)
        {
            await CreateConnectionAsync();
            if (_client == null)
            {
                throw new Exception($"{nameof(InventoryProtoService.InventoryProtoServiceClient)} initialization failed.");
            }
        }
    }

    private async Task CreateConnectionAsync()
    {
        var address = await _consulServiceDiscovery.GetServiceAddress("inventory");
        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _channel = GrpcChannel.ForAddress("https://" + address, new GrpcChannelOptions { HttpHandler = httpHandler });
        _client = new InventoryProtoService.InventoryProtoServiceClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}

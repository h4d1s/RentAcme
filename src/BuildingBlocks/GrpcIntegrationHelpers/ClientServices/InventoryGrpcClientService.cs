using Grpc.Core;
using Grpc.Net.Client;
using GrpcIntegrationHelpers.Models;
using Inventoryproto;

namespace GrpcIntegrationHelpers.ClientServices;

public class InventoryGrpcClientService : IInventoryGrpcClientService, IDisposable
{
    private readonly string _address = "https://inventory-api:8081";
    private GrpcChannel _channel = null!;
    private InventoryProtoService.InventoryProtoServiceClient _client = null!;

    public InventoryGrpcClientService()
    {
        CreateConnectionAsync();
    }

    public async Task<VehicleDto> GetVehicleAsync(Guid id)
    {
        if (string.IsNullOrEmpty(id.ToString()))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id is required."));
        }

        EnsureConnectionCreated();

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

        EnsureConnectionCreated();

        var request = new CheckVehicleRequest { VehicleId = id.ToString() };
        var reply = await _client.CheckVehicleAsync(request);

        return reply.IsExists;
    }

    private void EnsureConnectionCreated()
    {
        if (_client == null)
        {
            CreateConnectionAsync();
            if (_client == null)
            {
                throw new Exception($"{nameof(InventoryProtoService.InventoryProtoServiceClient)} initialization failed.");
            }
        }
    }

    private void CreateConnectionAsync()
    {
        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _channel = GrpcChannel.ForAddress(_address, new GrpcChannelOptions { HttpHandler = httpHandler });
        _client = new InventoryProtoService.InventoryProtoServiceClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}

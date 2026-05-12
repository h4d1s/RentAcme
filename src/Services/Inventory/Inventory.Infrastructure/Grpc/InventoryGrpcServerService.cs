using Grpc.Core;
using Inventory.Domain.AggregatesModel.VehicleAggregate;
using Inventoryproto;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Grpc;

public class InventoryGrpcServerService : InventoryProtoService.InventoryProtoServiceBase
{
    private readonly ILogger<InventoryGrpcServerService> _logger;
    private readonly IVehicleRepository _vehicleRepository;

    public InventoryGrpcServerService(
        ILogger<InventoryGrpcServerService> logger,
        IVehicleRepository vehicleRepository)
    {
        _logger = logger;
        _vehicleRepository = vehicleRepository;
    }

    public override async Task<GetVehicleReponse> GetVehicle(GetVehicleRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Checking vehicle {Id}", request.VehicleId);
        Vehicle? vehicle = null;

        try
        {
            var id = Guid.Parse(request.VehicleId);
            vehicle = await _vehicleRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return new GetVehicleReponse
        {
            RegistrationPlates = vehicle?.RegistrationPlates,
            RentalPricePerDay = (double)(vehicle?.RentalPricePerDay ?? 0),
            VariantId = vehicle?.VariantId.ToString(),
        };
    }

    public override async Task<VehicleStatusReponse> CheckVehicle(CheckVehicleRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Checking vehicle {Id}", request.VehicleId);
        var isExists = false;

        try
        {
            var id = Guid.Parse(request.VehicleId);
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            isExists = vehicle is not null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return new VehicleStatusReponse
        {
            IsExists = isExists
        };
    }
}

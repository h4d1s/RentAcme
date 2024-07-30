using Grpc.Core;
using Inventory.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventoryproto;
using Inventory.Domain.AggregatesModel.VehicleAggregate;

namespace Inventory.Infrastructure.Grpc;

public class InventoryGrpcServerService : InventoryProtoService.InventoryProtoServiceBase
{
    private readonly ILogger<InventoryGrpcServerService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryGrpcServerService(
        ILogger<InventoryGrpcServerService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public override async Task<GetVehicleReponse> GetVehicle(GetVehicleRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Checking vehicle {Id}", request.VehicleId);
        Vehicle vehicle = null;

        try
        {
            var id = Guid.Parse(request.VehicleId);
            vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
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
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            isExists = vehicle != null;
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

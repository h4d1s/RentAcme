using GrpcIntegrationHelpers.Models;

namespace GrpcIntegrationHelpers.ClientServices;

public interface IInventoryGrpcClientService
{
    public Task<bool> CheckIfExistsAsync(Guid id);
    public Task<VehicleDto> GetVehicleAsync(Guid id);
}

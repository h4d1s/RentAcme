using GrpcIntegrationHelpers.ClientServices;
using GrpcIntegrationHelpers.Models;

namespace Reservation.API.IntegrationTests.Fakes;

public sealed class FakeInventoryGrpcClientService : IInventoryGrpcClientService
{
    public Task<bool> CheckIfExistsAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<VehicleDto> GetVehicleAsync(Guid id)
    {
        return Task.FromResult(new VehicleDto {
            VariantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            RentalPricePerDay = 100,
            RegistrationPlates = "TEST-123"
        });
    }
}

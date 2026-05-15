using GrpcIntegrationHelpers.ClientServices;
using GrpcIntegrationHelpers.Models;

namespace Reservation.API.IntegrationTests.Fakes;

public sealed class FakeUserGrpcClientService : IUserGrpcClientService
{
    public Task<bool> CheckIfExistsAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<UserDto> GetUserAsync(Guid id)
    {
        return Task.FromResult(new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        });
    }

    public Task<UserDto> GetUserByExternalIdAsync(string externalId)
    {
        return Task.FromResult(new UserDto
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        });
    }
}
